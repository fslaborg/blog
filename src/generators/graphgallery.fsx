#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#load "layout.fsx"

open Graphgallerypostloader
open Layout
open Html
open System.IO

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =

    let graph_gallery_posts = 
        ctx.TryGetValues<GraphGalleryPost>() 
        |> Option.defaultValue Seq.empty
        |> List.ofSeq

    let groupedCategories =
        graph_gallery_posts
        |> List.groupBy (fun p -> p.post_config.category)

    Layout.layout ctx "Graph Gallery" [
        section [Class "hero is-small has-bg-darkmagenta"] [
            div [Class "hero-body"] [
                div [Class "container has-text-justified"] [
                    div [Class "main-TextField"] [
                        div [Class "media mb-4"] [
                            div [Class "media-left"] [
                                figure [Class "image is-128x128"] [
                                    img [Class "is-rounded" ; Src (Globals.prefixUrl "images/plotly-net.png")]
                                ]
                            ]
                            div [Class "media-content"] [
                                h1 [Class "title is-size-2 is-capitalized is-white is-inline-block is-strongly-emphasized-magenta mb-4"] [!! "Dotnet Graph Gallery"]
                            ]
                        ]
                        div [Class "block"] [
                            h1 [Class "subtitle is-size-4 is-white is-block"] [!! "Welcome to the Dotnet Graph gallery - a collection of blogposts and tutorials on creating all kinds of graphs with Plotly.NET in both F# and C#"]
                        ]
                    ]
                ]
            ]
        ] 
        section [] [
            div [Class "container has-text-justified"] [
                div [Class "main-TextField"] [
                    ul [] (
                        groupedCategories
                        |> List.map (fun (category, posts) ->
                            li [] [
                                h1 [Class "title is-darkmagenta is-size-2 is-emphasized-magenta"] [
                                    !! (category |> GraphCategory.toString)
                                    a [Href (Globals.prefixUrl $"graph-gallery/categories/{category}.html"); Class "is-aquamarine is-size-4"] [!! "[view timeline]"]
                                ]
                                h3 [Class "subtitle is-darkmagenta is-size-4"] [!! (category |> GraphCategory.getDescription)]
                                div [Class "columns"] [
                                    let mutable c1, c2, c3 = [], [], []
                                    posts
                                    |> List.iteri (fun i p ->
                                        match (i%3) with
                                        | 0 -> c1 <- c1@[p]
                                        | 1 -> c2 <- c2@[p]
                                        | _ -> c3 <- c3@[p]
                                    )
                                    div [if posts.Length > 1 then Class "column has-text-centered has-border-right-magenta is-4" else Class "column has-text-centered is-4"] [
                                        ul [] [
                                            for post in c1 -> 
                                                li [] [
                                                    a [Class "is-magenta is-size-5"; Href (Globals.prefixUrl (Path.Combine[|"graph-gallery"; snd post.file_names[0]|]))] [!! post.post_config.title]
                                                ]
                                        ]
                                    ]
                                    div [if posts.Length > 2 then Class "column has-text-centered has-border-right-magenta is-4" else Class "column has-text-centered is-4"] [
                                        ul [] [
                                            for post in c2 -> 
                                                li [] [
                                                    a [Class "is-magenta is-size-5"; Href (Globals.prefixUrl (Path.Combine[|"graph-gallery"; snd post.file_names[0]|]))] [!! post.post_config.title]
                                                ]
                                        ]
                                    ]
                                    div [Class "column has-text-centered is-4"] [
                                        ul [] [
                                            for post in c3 -> 
                                                li [] [
                                                    a [Class "is-magenta is-size-5"; Href (Globals.prefixUrl (Path.Combine[|"graph-gallery"; snd post.file_names[0]|]))] [!! post.post_config.title]
                                                ]
                                        ]
                                    ]
                                ]
                            ]
                        )

                    )
                ]
            ]
        ]
    ]
    |> Layout.render ctx