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

    Layout.layout ctx "Blog" [
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
                ]
            ]
        ]
    ]