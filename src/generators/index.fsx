#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"
#if !FORNAX
#load "../loaders/postloader.fsx"
#endif

open Postloader
open Html

let latest_post_display (latest_post: NotebookPost)=

    let latest_post_url = Globals.prefixUrl $"posts/{latest_post.file_name}"
    let latest_post_category_url = Globals.prefixUrl $"posts/categories/{latest_post.post_config.category}.html"

    div [Class "content"] [
        h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta"] [!!"Latest post"]
        div [Class "container"] [
            h3 [Class "subtitle mt-0"] [a [Href latest_post_url; Class "is-magenta"] [!! latest_post.post_config.title] ]
            !! " by "
            a [Href latest_post.post_config.author_link; Class "is-aquamarine"] [!! latest_post.post_config.author]
            !! "in "
            a [Href latest_post_category_url; Class "is-aquamarine"] [!! latest_post.post_config.category]
        ]
    ]

let browse_categories_display (posts: NotebookPost list) =
    div [Class "content"] [
        h1 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta"] [!!"Browse categories"]
        div [Class "container"] [
            ul [Class "mt-0"] (
                posts
                |> List.map (fun p -> p.post_config.category)
                |> List.distinct
                |> List.map (fun c -> 
                    let link = Globals.prefixUrl $"posts/categories/{c}.html"
                    li [] [h3 [Class "subtitle"] [a [Href link; Class "is-magenta"] [!! c] ]]
                )
            )
        ]
    ]


let generate' (ctx : SiteContents) (_: string) =

    let posts = 
        ctx.TryGetValues<NotebookPost>() 
        |> Option.defaultValue Seq.empty
        |> List.ofSeq

    let latest_post = posts |> List.minBy (fun p -> System.DateTime.Now.Ticks - p.post_config.date.Ticks)
    
    Layout.layout ctx "FsLab Blog" [
        section [Class "hero is-medium has-bg-magenta"] [
            div [Class "hero-body"] [
                div [Class "container has-text-justified"] [
                    div [Class "main-TextField"] [
                        div [Class "media mb-4"] [
                            div [Class "media-left"] [
                                figure [Class "image is-128x128"] [
                                    img [Id "logo-square"; Class "is-rounded" ; Src (Globals.prefixUrl "images/logo-rounded.svg")]
                                ]
                            ]
                            div [Class "media-content"] [
                                h1 [Class "main-title is-capitalized is-white is-inline-block is-strongly-emphasized-darkmagenta mb-4"] [!! "The FsLab Blog"]
                            ]
                        ]
                        div [Class "block"] [
                            h1 [Class "title is-size-3 is-capitalized is-white is-block"] [!! "Welcome to the FsLab glog!"]
                        ]
                        div [Class "content is-white is-size-4"] [
                            div [Class "block is-white"] [
                                p [] [!! "In this blog, FsLab contributors post content on all things FsLab."]
                                p [] [!! "You can find post in a wide range of topics - from getting a F# programming environment up and running over classic data science samples such as clustering the Iris data set or linear regression on the Boston housing dataset to advanced topics such as replicate quality control or q values."]
                                p [] [!! "Read the latest post below or browse all posts by categories."]
                            ]
                        ]
                    ]
                ]
            ]
        ] 
        section [] [
            div [Class "container has-text-justified"] [
                div [Class "main-TextField"] [
                    div [Class "columns"] [
                        div [Class "column is-6"] [
                            latest_post_display latest_post
                        ]
                        div [Class "column is-6"] [
                            browse_categories_display posts
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