#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Postloader
open Graphgallerypostloader
open Html

let latest_post_display (latest_post: NotebookPost) =

    div [Class "content"] [
        h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta is-size-4"] [!!"Latest post"]
        Layout.standardPostPreview latest_post
    ]

let browse_post_categories_display (posts: NotebookPost list) =
    div [Class "content"] [
        h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta is-size-4"] [!!"Browse post categories"]
        div [Class "container"] [
            ul [Class "mt-0"] (
                posts
                |> List.countBy (fun p -> p.post_config.category)
                |> List.map (fun (c,count) -> 
                    let link = Globals.prefixUrl $"posts/categories/{c}.html"
                    li [] [
                        h3 [Class "subtitle mb-1 is-size-4"] [a [Href link; Class "is-magenta"] [!! $"{c |> PostCategory.toString} [{count}]"] ]
                        p [Class "is-size-6"] [!! (c |> PostCategory.getDescription)]
                    ]
                )
            )
        ]
    ]

let latest_graph_gallery_post_display (latest_post: GraphGalleryPost) =

    div [Class "content"] [
        h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta is-size-4"] [!!"Latest post"]
        Layout.graphGalleryPostPreview latest_post
    ]

let browse_graph_gallery_post_categories_display (posts: GraphGalleryPost list) =
    div [Class "content"] [
        h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta is-size-4"] [!!"Browse graph categories"]
        div [Class "container"] [
            ul [Class "mt-0"] (
                posts
                |> List.countBy (fun p -> p.post_config.category)
                |> List.map (fun (c,count) -> 
                    let link = Globals.prefixUrl $"graph-gallery/categories/{c}.html"
                    li [] [
                        h3 [Class "subtitle mb-1 is-size-4"] [a [Href link; Class "is-magenta"] [!! $"{c |> GraphCategory.toString} [{count}]"] ]
                        p [Class "is-size-6"] [!! (c |> GraphCategory.getDescription)]
                    ]
                )
            )
        ]
    ]

let generate' (ctx : SiteContents) (_: string) =

    let posts = 
        ctx.TryGetValues<NotebookPost>() 
        |> Option.defaultValue Seq.empty
        |> List.ofSeq

    let graph_gallery_posts = 
        ctx.TryGetValues<GraphGalleryPost>() 
        |> Option.defaultValue Seq.empty
        |> List.ofSeq

    let latest_post = posts |> List.minBy (fun p -> System.DateTime.Now.Ticks - p.post_config.date.Ticks)
    let latest_graph_gallery_post = graph_gallery_posts |> List.minBy (fun p -> System.DateTime.Now.Ticks - p.post_config.date.Ticks)
    
    Layout.layout ctx "FsLab Blog" [
        section [Class "hero is-small has-bg-darkmagenta"] [
            div [Class "hero-body"] [
                div [Class "container has-text-justified"] [
                    div [Class "main-TextField"] [
                        div [Class "media mb-4"] [
                            div [Class "media-left"] [
                                figure [Class "image is-128x128"] [
                                    img [Id "package-header-img"; Class "is-rounded" ; Src (Globals.prefixUrl "images/skills.svg")]
                                ]
                            ]
                            div [Class "media-content"] [
                                h1 [Class "title is-size-1 is-capitalized is-white is-inline-block is-strongly-emphasized-aquamarine mb-4"] [!! "The FsLab Blog"]
                            ]
                        ]
                        div [Class "block"] [
                            h1 [Class "subtitle is-size-4 is-white is-block"] [!! "Welcome to the FsLab blog, where community members post all kinds of F# data science content."]
                        ]
                    ]
                ]
            ]
        ] 
        section [] [
            div [Class "container has-text-justified"] [
                div [Class "main-TextField"] [
                    h1 [Class "title is-size-2 is-darkmagenta is-emphasized-magenta is-inline-block"] [!! "FsLab blog"]
                    h3 [Class "subtitle"] [!! "The FsLab blog is a diverse collection of datascience content, mainly focused on F#."]
                    div [Class "columns"] [
                        div [Class "column is-6"] [
                            latest_post_display latest_post
                        ]
                        div [Class "column is-6"] [
                            browse_post_categories_display posts
                        ]
                    ]
                    hr [Class "has-bg-darkmagenta"]
                ]
            ]
        ]
        section [] [
            div [Class "container has-text-justified"] [
                div [Class "main-TextField"] [
                    h1 [Class "title is-size-2 is-darkmagenta is-emphasized-magenta is-inline-block"] [!! "Dotnet Graph Gallery"]
                    h3 [Class "subtitle"] [!! "The Dotnet Graph Gallery contains posts about all kinds of data visualizations using Plotly.NET with both F# and C#."]
                    div [Class "columns"] [
                        div [Class "column is-6"] [
                            latest_graph_gallery_post_display latest_graph_gallery_post
                        ]
                        div [Class "column is-6"] [
                            browse_graph_gallery_post_categories_display graph_gallery_posts
                        ]
                    ]
                ]
            ]
        ]
    ]

// For a generator file to be used in fornax (meaning as a generator in the Fornax config in `config.fsx`),
// it must always end with a `generate` function that returns a string (which is the content that gets written to a file)
let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    printfn "[index.fsx] Starting generate function ..."
    generate' ctx page
    |> Layout.render ctx