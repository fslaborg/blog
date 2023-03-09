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
    |> List.groupBy (fun post -> post.post_config.category)
    |> List.map (fun (category, posts) ->
        Path.Combine([|projectRoot; "_public"; "posts"; "categories"; $"{category}.html"|]),
        Layout.layout ctx "Posts" [
            h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta"] [!! $"{category} posts"]
            div [Class "container"] [
                ul [Class "mt-0"] (
                    posts
                    |> List.sortByDescending (fun p -> p.post_config.date)
                    |> List.map (fun post -> 
                        let post_url = Globals.prefixUrl $"posts/{post.file_name}"
                        li [] [
                            a [Href post_url; Class "is-magenta"] [!! post.post_config.title]
                            !! " by "
                            a [Href post.post_config.author_link; Class "is-aquamarine"] [!! post.post_config.author]
                        ]
                    )
                )
            ]
        ]
        |> Layout.render ctx
    )
