#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#load "layout.fsx"

open Graphgallerypostloader
open Layout
open Html

open System.IO
open System.Diagnostics
open System.Text.RegularExpressions


let generate (ctx : SiteContents) (projectRoot: string) (page: string) =

    let posts = 
        ctx.TryGetValues<GraphGalleryPost>() 
        |> Option.defaultValue Seq.empty
        |> List.ofSeq

    posts
    |> List.map (fun post ->
        let full_paths = post.original_paths
        let tmp_path = Path.GetTempPath()

        let fileNames = [|for (language,full_path) in full_paths -> language, Path.GetFileName(full_path)|]
        let html_filenames = fileNames |> Array.map (fun (lang,fileName) -> lang, fileName.Replace("ipynb","html"))
        
        /// save temporary notebooks to convert here
        let fixed_nb_files =  fileNames |> Array.map (fun (lang,fileName) -> lang, Path.Combine(tmp_path, fileName))
        /// save nbconvert result here temporarily
        let nbconvert_temp_output_files = html_filenames |> Array.map (fun (lang, html_filename) -> lang, Path.Combine(tmp_path, html_filename))

        Array.iter2 (fun (lang,source_path) (lang2, target_path) -> if lang = lang2 then Globals.fixNotebookJson lang source_path target_path else failwith "lol?") full_paths fixed_nb_files

        let results = 
            fixed_nb_files
            |> Array.mapi (fun i (language, fixed_notebook) ->
                printfn $"[graph gallery generator]: starting jupyter --output-dir='{tmp_path}' nbconvert --to html {fixed_notebook}"
                
                let psi = ProcessStartInfo()
                psi.FileName <- "jupyter"
                psi.Arguments <- $"nbconvert --output-dir='{tmp_path}' --to html {fixed_notebook}"
                psi.CreateNoWindow <- true
                psi.WindowStyle <- ProcessWindowStyle.Hidden
                psi.UseShellExecute <- true
                try
                    let proc = Process.Start psi
                    proc.WaitForExit()
                    let notebook_content = File.ReadAllText (snd nbconvert_temp_output_files[i])
                    
                    File.Delete (snd nbconvert_temp_output_files[i])
                    File.Delete fixed_notebook

                    let processed_notebook = Globals.processConvertedNotebook notebook_content
                    let toc = Globals.getNotebookTOC processed_notebook
 
                    let heroLanguageSelection = [
                        div [Class "container"] [
                            h1 [Class "subtitle is-size-4 is-white"] [!! "select language:"]
                            div [Class "tags"] (
                                post.html_paths
                                |> Array.toList 
                                |> List.mapi (fun i (l, path) ->
                                    a [
                                        if language = l then Class "tag is-language is-active is-large" else Class "tag is-language is-large"
                                        Href (Globals.prefixUrl $"graph-gallery/{snd post.file_names[i]}")
                                    ] [!! l]
                                )
                            )
                        ]
                    ]

                    let content = 
                        Layout.graphGalleryPostLayout ctx post.post_config toc "Graph Gallery" heroLanguageSelection [
                            div [
                                Class "content jp-Notebook"
                                HtmlProperties.Custom ("data-jp-theme-light","true")
                                HtmlProperties.Custom ("data-jp-theme-name","JupyterLab Light")
                            ] [!!processed_notebook]
                        ]
                        |> Layout.render ctx

                    printfn $"[graph gallery generator]: generated {snd post.html_paths[i]}"
                    (
                        snd post.html_paths[i],
                        content
                    )
                with
                | ex ->
                    printfn "[graph gallery generator] EX: %s" ex.Message
                    printfn "make sure to set up a conda distribution with nbconvert installed."
                    "",
                    Layout.layout ctx "" List.empty
                    |> Layout.render ctx

            )
            |> Array.toList

        results

    )
    |> List.concat