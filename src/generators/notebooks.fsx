#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#load "layout.fsx"
#if !FORNAX
#load "../loaders/postloader.fsx"
#endif

open Postloader
open Layout
open Html

open System.IO
open System.Diagnostics


let generate (ctx : SiteContents) (projectRoot: string) (page: string) =

    ctx.TryGetValues<NotebookPost>() 
    |> Option.defaultValue Seq.empty
    |> List.ofSeq
    |> List.map (fun post -> 
        let content = File.ReadAllText(post.original_path)
        let new_path = post.html_path.Replace(".html", ".ipynb")

        new_path, content
    )

