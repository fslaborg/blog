#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"
#load "../globals.fsx"
open System.IO
open Markdig

type PostConfig = {
    title: string
    author: string
    author_link: string
    category: string
    date: System.DateTime
} with
    static member create(title, author, author_link, category, date) =
        {
            title = title
            author = author
            author_link = author_link
            category = category
            date = date
        }
    static member ofMap(m:Map<string,string>) =
        PostConfig.create(
            title = m["title"],
            author = m["author"],
            author_link = m["author_link"],
            category = m["category"],
            date = (System.DateTime.ParseExact(m["date"],"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture))
        )

let markdownPipeline =
    MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .Build()

let isSeparator (input : string) =
    input.StartsWith "---"

///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns content of config that should be used for the page
let getConfig (fileContent : string) =
    let fileContent = fileContent.Split System.Environment.NewLine
    let fileContent = fileContent |> Array.skip 1 //First line must be ---
    let indexOfSeperator = fileContent |> Array.findIndex isSeparator
    let splitKey (line: string) =
        let seperatorIndex = line.IndexOf(':')
        if seperatorIndex > 0 then
            let key = line.[.. seperatorIndex - 1].Trim().ToLower()
            let value = line.[seperatorIndex + 1 ..].Trim()
            Some(key, value)
        else
            None
    fileContent
    |> Array.splitAt indexOfSeperator
    |> fst
    |> Seq.choose splitKey
    |> Map.ofSeq

type NotebookPost = {
    file_name: string
    post_config: PostConfig
    html_path: string
    original_path: string
} with
    static member create(file_name, post_config, html_path, original_path) =
        {
            file_name = file_name
            post_config = post_config
            html_path = html_path
            original_path = original_path
        }

let loader (projectRoot: string) (siteContent: SiteContents) =
    let notebookRootPath = Path.Combine(projectRoot, "posts")

    Directory.EnumerateDirectories(notebookRootPath)
    |> Array.ofSeq
    |> Array.iter (fun (post_folder:string) ->

        let post_path = Path.Combine(post_folder,"post.ipynb")
        let config_path = Path.Combine(post_folder,"post_config.md")

        if not (File.Exists(post_path)) then failwith $"no post.ipynb in {post_folder}"
        if not (File.Exists(config_path)) then failwith $"no post_config.md in {post_folder}"

        let post_config =
            config_path
            |> File.ReadAllText
            |> getConfig
            |> PostConfig.ofMap

        let postName =
            post_path
                .Replace(notebookRootPath, "")
                .Replace("\\","/")
                .Replace("/post.ipynb",".html")
                .Replace("/","")

        let post =
            NotebookPost.create(
                file_name = postName,
                post_config = post_config, 
                html_path = Path.Combine([|projectRoot; "_public"; "posts"; postName|]), 
                original_path = post_path
            )

        // printfn "%A" post

        siteContent.Add(post)
        printfn $"[post loader] loaded post from {post.original_path}"
    )
        
    siteContent