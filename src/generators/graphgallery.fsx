#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#load "layout.fsx"

open Graphgallerypostloader
open Layout
open Html


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
                                h1 [Class "title is-darkmagenta is-size-2 is-emphasized-magenta"] [!! (category |> GraphCategory.toString)]
                                h3 [Class "subtitle is-darkmagenta is-size-4"] [!! (category |> GraphCategory.getDescription)]
                                ul [] [
                                    for post in posts -> 
                                        li [] [
                                            a [Class ""; Href (Globals.prefixUrl $"graph-gallery/{snd post.file_names[0]}")] [!! post.post_config.title]
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