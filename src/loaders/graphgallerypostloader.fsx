#r "../_lib/Fornax.Core.dll"
#r "../_lib/Markdig.dll"
#load "../globals.fsx"
open System.IO
open Markdig

type GraphCategory =
| Basic
| Distribution
| Finance
| Chart3D
| Map
| Special
| Other of string

    static member ofString (s:string) =
        match s.Trim().ToLower() with
        | "basic" -> Basic
        | "distribution" -> Distribution
        | "finance" -> Finance
        | "3d" -> Chart3D
        | "map" -> Map
        | "special" -> Special
        | _ -> Other s

    static member toString (pc: GraphCategory) =
        match pc with
        | Basic        -> "Basic Graphs"
        | Distribution -> "Distribution Graphs"
        | Finance      -> "Finance Graphs"
        | Chart3D      -> "3-Dimensional Graphs"
        | Map          -> "Geospecial Graphs"
        | Special      -> "Specialized Graphs"
        | Other o      -> o

    static member getDescription (pc: GraphCategory) =
        match pc with
        | Basic        -> "Simple, easy to understand graphs"
        | Distribution -> "Graphs visualizing distributions of features, e.g. frequencies of occurances of observations"
        | Finance      -> "Graphs for financial analysis"
        | Chart3D      -> "Three dimensional graphs"
        | Map          -> "Graphs for geospatial data"
        | Special      -> "Specialized charts that do not directly fit into the other categories"
        | Other o      -> o

type GraphGalleryPostConfig = {
    title: string
    author: string
    author_link: string
    category: GraphCategory
    date: System.DateTime
    preview_image: string option
    summary: string option
    last_updated_on: System.DateTime option
    last_updated_by: string option
    last_updated_by_link: string option
} with
    static member create(title, author, author_link, category, date, ?preview_image, ?summary, ?last_updated_on, ?last_updated_by, ?last_updated_by_link) =
        {
            title = title
            author = author
            author_link = author_link
            category = category
            date = date
            preview_image = preview_image
            summary = summary
            last_updated_on = last_updated_on
            last_updated_by = last_updated_by
            last_updated_by_link = last_updated_by_link
        }
    static member ofMap (source:string) (m:Map<string,string>) =

        let mandatoryFieldMissing (fieldName: string) (source:string) (o:string Option) = if o.IsNone then failwith $"missing field '{fieldName}' in config from {source}" else o.Value
        let parseDateString(dateString: string) = 
            try 
                System.DateTime.ParseExact(dateString,"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture)
            with _ ->
                failwith $"wrong date format in config from {source}, make sure to use YYY-MM-DD"

        let title = 
            m 
            |> Map.tryFind "title" 
            |> mandatoryFieldMissing "title" source
            |> fun t -> 
                let t = if t.StartsWith("\"") then t[1..] else t
                let t = if t.EndsWith("\"") then t[0..(t.Length-2)] else t
                t

        let author = m.TryFind "author" |> mandatoryFieldMissing "author" source
        let author_link = m.TryFind "author_link" |> mandatoryFieldMissing "author_link" source
        let category = 
            let tmp = m.TryFind "graph_category" |> mandatoryFieldMissing "graph_category" source
            try 
                GraphCategory.ofString tmp
            with _ ->
                failwith $"wrong post category format in config from {source}"
        let date = 
            m.TryFind "date" |> mandatoryFieldMissing "date" source |> parseDateString

        let preview_image = m |> Map.tryFind "preview_image" 
        let summary = m |> Map.tryFind "summary" 

        let last_updated_on = m.TryFind "last_updated_on" |> Option.map parseDateString
        let last_updated_by = m.TryFind "last_updated_by"
        let last_updated_by_link = m.TryFind "last_updated_by_link"        

        GraphGalleryPostConfig.create(
            title = title,
            author = author,
            author_link = author_link,
            category = category,
            date = date,
            ?preview_image = preview_image,
            ?summary = summary,
            ?last_updated_on = last_updated_on,
            ?last_updated_by = last_updated_by,
            ?last_updated_by_link = last_updated_by_link
        )

type GraphGalleryPost = {
    file_names: (string*string) []
    post_config: GraphGalleryPostConfig
    html_paths: (string*string) []
    original_paths: (string*string) []
} with
    static member create(file_names, post_config, html_paths, original_paths) =
        {
            file_names = file_names
            post_config = post_config
            html_paths = html_paths
            original_paths = original_paths
        }

let loader (projectRoot: string) (siteContent: SiteContents) =
    let graphGalleryRootPath = Path.Combine(projectRoot, "graph-gallery")

    Directory.EnumerateDirectories(graphGalleryRootPath)
    |> Array.ofSeq
    |> Array.iter (fun (graph_folder:string) ->

        let notebooks = 
            Directory.GetFiles(graph_folder,"*.ipynb",SearchOption.AllDirectories)
            |> Array.map (fun f -> Path.GetFileNameWithoutExtension(f), f)
        let config_path = Path.Combine(graph_folder,"graph_post_config.md")

        if notebooks.Length = 0 then failwith $"no posts found in {graph_folder}"

        for (language,notebook) in notebooks do
            if language <> "fsharp" && language <> "csharp" then failwith $"unsupported language {language} in {notebook}, supported names are fsharp.ipynb and csharp.ipynb"
        
        if not (File.Exists(config_path)) then failwith $"no post_config.md in {graph_folder}"

        let post_config =
            config_path
            |> File.ReadAllText
            |> Globals.getFrontmatter
            |> GraphGalleryPostConfig.ofMap config_path

        let postNames =
            notebooks
            |> Array.map (fun (language,path) ->
                let name = 
                    path
                        .Replace(graphGalleryRootPath, "")
                        .Replace("\\","/")
                        .Replace(".ipynb",".html")

                language, name[1..]
            )

        let graph_post =
            GraphGalleryPost.create(
                file_names = postNames,
                post_config = post_config, 
                html_paths = (
                    postNames 
                    |> Array.map (fun (language, name) -> 
                        language, Path.Combine([|projectRoot; "_public"; "graph-gallery"; yield! name.Split("/")|])
                    )
                ),
                original_paths = notebooks
            )

        // printfn "%A" post
        let languages = postNames |> Array.map fst |> String.concat ", "
        siteContent.Add(graph_post)
        printfn $"[graph gallery loader] loaded | {languages} | graph posts from {graph_folder}"
    )
        
    siteContent