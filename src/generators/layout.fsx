#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#if !FORNAX
#load "../loaders/pageloader.fsx"
#load "../loaders/postloader.fsx"
#load "../loaders/graphgallerypostloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html
open Postloader
open Graphgallerypostloader
open FsLab.Fornax

// Can be used to set the active item color in the navbar to the same color as the hero to get a bookmark effect
let getBgColorForActiveItem (siteTitle:string) =
    match siteTitle with
    | "Blog" -> "is-active-link-darkmagenta"
    | "Graph Gallery" -> "is-active-link-darkmagenta"
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
            link [Rel "stylesheet"; Href (Globals.prefixUrl "style/custom.css")]
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

let postLayout (ctx : SiteContents) (post_category:string) (post_category_url:string) (post_title:string) (post_date:System.DateTime) (post_author:string) (post_author_link: string) (toc:HtmlElement) active heroCnt bodyCnt =
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

    html [] [
        head [] [
            yield! Components.DefaultHeadTags ttl
            script [Src (Globals.prefixUrl "js/navbar.js")] []
            script [Src (Globals.prefixUrl "js/prism.js")] []
            link [Rel "stylesheet"; Href "https://cdn.jsdelivr.net/npm/bulma-timeline@3.0.5/dist/css/bulma-timeline.min.css"]
            link [Rel "stylesheet"; Href (Globals.prefixUrl "style/notebook.css")]
            link [Rel "stylesheet"; Href (Globals.prefixUrl "style/custom.css")]
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
                                    h1 [Class "title is-capitalized is-white is-inline-block is-emphasized-magenta mb-4"] [!! post_title]
                                    div [Class "block"] [
                                        h3 [Class "subtitle is-white is-block"] [
                                            !! $"Posted on {post_date.Year}-{post_date.Month}-{post_date.Day} by"
                                            a [Href post_author_link; Class "is-aquamarine"] [!! post_author]
                                            !! $" in "
                                            a [Href post_category_url; Class "is-aquamarine"] [!! (post_category)]
                                        ]
                                    ]
                                    yield! heroCnt
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

let standardPostLayout (ctx: SiteContents) (post_config: Postloader.PostConfig) (toc:HtmlElement) active bodyCnt =
    postLayout
        ctx
        (post_config.category |> PostCategory.toString)
        (Globals.prefixUrl $"posts/categories/{post_config.category}.html")
        post_config.title
        post_config.date
        post_config.author
        post_config.author_link
        toc
        active
        []
        bodyCnt

let graphGalleryPostLayout (ctx: SiteContents) (post_config: Graphgallerypostloader.GraphGalleryPostConfig) (toc:HtmlElement) active heroCnt bodyCnt =
    postLayout
        ctx
        (post_config.category |> GraphCategory.toString)
        (Globals.prefixUrl $"graph-gallery/categories/{post_config.category}.html")
        post_config.title
        post_config.date
        post_config.author
        post_config.author_link
        toc
        active
        heroCnt
        bodyCnt

let postPreview (preview_image_url: string option) (post_summary: string option) (post_url:string) (post_category_url:string) (post_category:string) (post_title:string) (post_date: System.DateTime) (post_author: string) (post_author_link: string) (tags:(string*string) list) =

    let has_image = Option.isSome preview_image_url
    let has_summary = Option.isSome post_summary

    div [Class "card pt-2"] [ 
        if has_image then 
            div [Class "card-image"] [
                a [Href post_url] [
                    figure [Class "image"] [
                        img [Src (Globals.prefixUrl preview_image_url.Value); Alt "post preview image"] 
                    ]
                ]
            ]
        div [Class "card-header is-emphasized-darkmagenta"] [
            h1 [Class "card-header-title is-size-4"] [a [Href post_url; Class "is-magenta"] [!!post_title]]
        ]
        div [Class "card-content is-size-6"] [
            if has_summary then div [Class "content"] [!!post_summary.Value]
            if tags.Length > 0 then
                div [Class "tags"] (
                    tags
                    |> List.map (fun (t,l) ->
                        a [Class "tag is-language is-active"; Href l] [!! t]
                    )
                )
            !! $"Posted on {post_date.Year}-{post_date.Month}-{post_date.Day} by "
            a [Href post_author_link; Class "is-aquamarine"] [!! post_author]
            !! "in "
            a [Href post_category_url; Class "is-aquamarine"] [!! (post_category)]
        ]
    ]


let standardPostPreview (post: Postloader.NotebookPost) =
    postPreview
        post.post_config.preview_image
        post.post_config.summary
        (Globals.prefixUrl $"posts/{post.file_name}")
        (Globals.prefixUrl $"posts/categories/{post.post_config.category}.html")
        (post.post_config.category |> PostCategory.toString)
        post.post_config.title
        post.post_config.date
        post.post_config.author
        post.post_config.author_link
        []

let graphGalleryPostPreview (post: GraphGalleryPost) =
    postPreview
        post.post_config.preview_image
        post.post_config.summary
        (Globals.prefixUrl $"graph-gallery/{snd post.file_names[0]}")
        (Globals.prefixUrl $"graph-gallery/categories/{post.post_config.category}.html")
        (post.post_config.category |> GraphCategory.toString)
        post.post_config.title
        post.post_config.date
        post.post_config.author
        post.post_config.author_link
        (post.file_names |> Array.toList |> List.map (fun (lang, path) -> lang, Globals.prefixUrl $"graph-gallery/{path}"))

let render (ctx : SiteContents) cnt =
  cnt
  |> HtmlElement.ToString
#if WATCH
  |> Globals.injectWebsocketCode 
#endif