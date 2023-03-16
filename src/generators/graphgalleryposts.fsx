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
        let tmp_paths = [|for (language,_) in full_paths -> language, Path.GetTempPath ()|]
        let tmp_outputs = [|for (language,full_path) in full_paths -> language, Path.GetFileName(full_path).Replace("ipynb","html")|]
        let output_paths = Array.map2 (fun (l1,tmp_path) (l2,tmp_output) -> if l1 = l2 then l1, Path.Combine(tmp_path, tmp_output) else failwith "lol?") tmp_paths tmp_outputs

        full_paths |> Array.iter (fun (lang,path) -> Globals.fixNotebookJson lang path)
        
        let results = 
            tmp_paths
            |> Array.mapi (fun i tmp_path ->
                printfn $"[graph gallery generator]: starting jupyter --output-dir='{snd tmp_path}' nbconvert --to html {snd full_paths[i]}"
                
                let psi = ProcessStartInfo()
                psi.FileName <- "jupyter"
                psi.Arguments <- $"nbconvert --output-dir='{snd tmp_path}' --to html {snd full_paths[i]}"
                psi.CreateNoWindow <- true
                psi.WindowStyle <- ProcessWindowStyle.Hidden
                psi.UseShellExecute <- true
                try
                    let proc = Process.Start psi
                    proc.WaitForExit()
                    let notebook_content = File.ReadAllText (snd output_paths[i])
                    File.Delete (snd output_paths[i])
                    let processed_notebook = Globals.processConvertedNotebook notebook_content
                    let toc = Globals.getNotebookTOC processed_notebook
 
                    let content = 
                        Layout.graphGalleryPostLayout ctx post.post_config toc "Posts" [
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