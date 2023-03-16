#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#load "layout.fsx"

open Postloader
open Layout
open Html

open System.IO
open System.Diagnostics
open System.Text.RegularExpressions

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =

    let posts = 
        ctx.TryGetValues<NotebookPost>() 
        |> Option.defaultValue Seq.empty
        |> List.ofSeq

    posts
    |> List.map (fun post ->
        let full_path = post.original_path
        let tmp_path = Path.GetTempPath ()
        let tmp_output = Path.GetFileName(full_path).Replace("ipynb","html")
        let output_path = Path.Combine(tmp_path, tmp_output)

        full_path |> Globals.fixNotebookJson "fsharp"

        printfn $"[post generator]: starting jupyter --output-dir='{tmp_path}' nbconvert --to html {full_path}"
        let psi = ProcessStartInfo()
        psi.FileName <- "jupyter"
        psi.Arguments <- $"nbconvert --output-dir='{tmp_path}' --to html {full_path}"
        psi.CreateNoWindow <- true
        psi.WindowStyle <- ProcessWindowStyle.Hidden
        psi.UseShellExecute <- true
        try
            let proc = Process.Start psi
            proc.WaitForExit()
            let notebook_content = File.ReadAllText output_path
            File.Delete output_path

            let processed_notebook = Globals.processConvertedNotebook notebook_content
            let toc = Globals.getNotebookTOC processed_notebook 

            let content = 
                Layout.standardPostLayout ctx post.post_config toc "Blog" [
                    div [
                        Class "content jp-Notebook"
                        HtmlProperties.Custom ("data-jp-theme-light","true")
                        HtmlProperties.Custom ("data-jp-theme-name","JupyterLab Light")
                    ] [!!processed_notebook]
                ]
                |> Layout.render ctx

            printfn $"[post generator]: generated {post.html_path}"
            (
                post.html_path,
                content
            )
        with
        | ex ->
            printfn "[post generator] EX: %s" ex.Message
            printfn "make sure to set up a conda distribution with nbconvert installed."
            "",
            Layout.layout ctx "" List.empty
            |> Layout.render ctx
    )