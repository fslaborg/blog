#load "../globals.fsx"
#r "../_lib/Fornax.Core.dll"

/// Contains page metadata
type Page = {
    title: string
    link: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =

    siteContent.Add({title = "Home"; link = "https://fslab.org"})
    siteContent.Add({title = "Data science packages"; link = "https://fslab.org/packages.html"})
    siteContent.Add({title = "Blog"; link = Globals.prefixUrl ""})
    siteContent.Add({title = "Graph Gallery"; link = Globals.prefixUrl "graph-gallery.html"})

    siteContent
