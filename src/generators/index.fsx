#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let generate' (ctx : SiteContents) (_: string) =
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
                                h1 [Class "main-title is-capitalized is-white is-inline-block is-strongly-emphasized-darkmagenta mb-4"] [!! "FsLab Blog"]
                            ]
                        ]
                        div [Class "block"] [
                            h1 [Class "title is-size-3 is-capitalized is-white is-block"] [!! "Welcome to the FsLab template for fornax!"]
                        ]
                        div [Class "content is-white is-size-4"] [
                            div [Class "block is-white"] [
                                !! "This is how the blank template looks like. Flashy but boring! Get started by adding some sections below for your project FsLab Blog!"
                            ]
                        ]
                    ]
                ]
            ]
        ]
        section [] [
            div [Class "container has-text-justified"] [
                div [Class "main-TextField"] [
                    h1 [Class "title"] [!!"here is some code, isn't that nice?"]
                ]
            ]
            div [Class "container"] [
                div [Class "main-TextField"] [
                    code [Class "language-fsharp"] [
                        !! "printfn \"hello world\""
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