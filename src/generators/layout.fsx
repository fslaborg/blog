#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#if !FORNAX
#load "../loaders/pageloader.fsx"
#load "../loaders/postloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html
open Postloader
open FsLab.Fornax

// Can be used to set the active item color in the navbar to the same color as the hero to get a bookmark effect
let getBgColorForActiveItem (siteTitle:string) =
    match siteTitle with
    | "FsLab Blog" -> "is-active-link-magenta"
    | _ -> siteTitle


/// The main html skeleton generation happens here.
/// This function embeds `bodyCnt` into the template's html layout.
/// Use `active` to control the color of the active menu entry.
let layout (ctx : SiteContents) active bodyCnt =
    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let ttl =
        siteInfo
        |> Option.map (fun si -> si.title)
        |> Option.defaultValue ""

    let menuEntries =
        pages
        |> Seq.map (fun p ->
            let cls = if p.title = active then (sprintf "navbar-item %s smooth-hover" (getBgColorForActiveItem active)) else "navbar-item"
            a [Class cls; Href p.link] [!! p.title ]
        )
        |> Seq.toList

    html [Class "has-navbar-fixed-top"] [
        head [] [
            yield! Components.DefaultHeadTags ttl
            script [Src (Globals.prefixUrl "js/navbar.js")] []
            script [Src (Globals.prefixUrl "js/prism.js")] []
            link [Rel "stylesheet"; Href "https://cdn.jsdelivr.net/npm/bulma-timeline@3.0.5/dist/css/bulma-timeline.min.css"]
            link [Rel "stylesheet"; Href (Globals.prefixUrl "style/notebook.css")]
        ]
        body [] [
            Components.Navbar(
                LogoSource = (Globals.prefixUrl "images/favicon.png"),
                LogoLink = "https://fslab.org",
                MenuEntries = menuEntries,
                SocialLinks = [
                    a [Class "navbar-item is-magenta"; Href "https://twitter.com/fslaborg"] [Components.Icon "fab fa-twitter"]
                    a [Class "navbar-item is-magenta"; Href "https://github.com/fslaborg"] [Components.Icon "fab fa-github"]
                ]
            )
            yield! bodyCnt
        ]
        Components.Footer()
    ]

let postLayout (ctx : SiteContents) (post_config:PostConfig) (toc:HtmlElement) active bodyCnt =
    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let ttl =
        siteInfo
        |> Option.map (fun si -> si.title)
        |> Option.defaultValue ""

    let menuEntries =
        pages
        |> Seq.map (fun p ->
            let cls = if p.title = active then (sprintf "navbar-item %s smooth-hover" (getBgColorForActiveItem active)) else "navbar-item"
            a [Class cls; Href p.link] [!! p.title ]
        )
        |> Seq.toList

    let category_url = Globals.prefixUrl $"posts/categories/{post_config.category}.html"

    html [] [
        head [] [
            yield! Components.DefaultHeadTags ttl
            script [Src (Globals.prefixUrl "js/navbar.js")] []
            script [Src (Globals.prefixUrl "js/prism.js")] []
            link [Rel "stylesheet"; Href "https://cdn.jsdelivr.net/npm/bulma-timeline@3.0.5/dist/css/bulma-timeline.min.css"]
            link [Rel "stylesheet"; Href (Globals.prefixUrl "style/notebook.css")]
        ]
        body [] [
            div [Class "columns is-fullheight m-0"] [
                div [Class "column is-2 is-paddingless box m-0"] [
                    aside [Class "menu p-4"; Id "graph-menu"] [
                        div [Class "content"] [
                            h3 [Class "title is-capitalized is-inline-block is-emphasized-darkmagenta mb-4"] [!! "Table of contents"]
                            toc
                        ]
                    ]
                ]
                div [Class "column is-10 is-paddingless pl-1 pr-6"] [
                    Components.Navbar(
                        IsFixed = false,
                        LogoSource = (Globals.prefixUrl "images/favicon.png"),
                        LogoLink = "https://fslab.org",
                        MenuEntries = menuEntries,
                        SocialLinks = [
                            a [Class "navbar-item is-magenta"; Href "https://twitter.com/fslaborg"] [Components.Icon "fab fa-twitter"]
                            a [Class "navbar-item is-magenta"; Href "https://github.com/fslaborg"] [Components.Icon "fab fa-github"]
                        ]
                    )
                    section [Class "hero is-small has-bg-darkmagenta"] [
                        div [Class "hero-body"] [
                            div [Class "container has-text-justified"] [
                                div [Class "main-TextField"] [
                                    h1 [Class "title is-capitalized is-white is-inline-block is-emphasized-magenta mb-4"] [!! post_config.title]
                                    div [Class "block"] [
                                        h3 [Class "subtitle is-white is-block"] [
                                            !! $"Posted on {post_config.date.Year}-{post_config.date.Month}-{post_config.date.Day} by"
                                            a [Href post_config.author_link; Class "is-aquamarine"] [!! post_config.author]
                                            !! $" in "
                                            a [Href category_url; Class "is-aquamarine"] [!! (post_config.category |> PostCategory.toString)]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                    yield! bodyCnt
                ]
            ]
        ]
        Components.Footer()
    ]

let postPreview (post:NotebookPost) =
    let has_image = post.post_config.preview_image.IsSome
    let has_summary = post.post_config.summary.IsSome

    let post_url = Globals.prefixUrl $"posts/{post.file_name}"
    let post_category_url = Globals.prefixUrl $"posts/categories/{post.post_config.category}.html"

    div [Class "card pt-2"] [ 
        if has_image then 
            div [Class "card-image"] [
                a [Href post_url] [
                    figure [Class "image"] [
                        img [Src (Globals.prefixUrl post.post_config.preview_image.Value); Alt "post preview image"] 
                    ]
                ]
            ]
        div [Class "card-header is-emphasized-darkmagenta"] [
            h1 [Class "card-header-title is-size-4"] [a [Href post_url; Class "is-magenta"] [!!post.post_config.title]]
        ]
        div [Class "card-content is-size-6"] [
            if has_summary then div [Class "content"] [!!post.post_config.summary.Value]
            !! $"Posted on {post.post_config.date.Year}-{post.post_config.date.Month}-{post.post_config.date.Day} by "
            a [Href post.post_config.author_link; Class "is-aquamarine"] [!! post.post_config.author]
            !! "in "
            a [Href post_category_url; Class "is-aquamarine"] [!! (post.post_config.category |> PostCategory.toString)]
        ]
    ]

let render (ctx : SiteContents) cnt =
  cnt
  |> HtmlElement.ToString
#if WATCH
  |> Globals.injectWebsocketCode 
#endif