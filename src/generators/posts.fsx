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

        let fileName = Path.GetFileName(full_path)
        let html_filename = fileName.Replace("ipynb","html")

        /// save temporary notebook to convert here
        let fixed_nb_file = Path.Combine(tmp_path, fileName)
        /// save nbconvert result here temporarily
        let nbconvert_temp_output_file = Path.Combine(tmp_path, html_filename)

        Globals.fixNotebookJson "fsharp" full_path fixed_nb_file

        printfn $"[post generator]: starting jupyter --output-dir='{tmp_path}' nbconvert --to html {fixed_nb_file}"
        let psi = ProcessStartInfo()
        psi.FileName <- "jupyter"
        psi.Arguments <- $"nbconvert --output-dir='{tmp_path}' --to html {fixed_nb_file}"
        psi.CreateNoWindow <- true
        psi.WindowStyle <- ProcessWindowStyle.Hidden
        psi.UseShellExecute <- true
        try
            let proc = Process.Start psi
            proc.WaitForExit()
            let notebook_content = File.ReadAllText nbconvert_temp_output_file

            File.Delete nbconvert_temp_output_file
            File.Delete fixed_nb_file

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