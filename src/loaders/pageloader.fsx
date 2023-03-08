#load "../globals.fsx"
#r "../_lib/Fornax.Core.dll"

/// Contains page metadata
type Page = {
    title: string
    link: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =

    // currently, only the Home page (index.fsx) is there
    siteContent.Add({title = "FsLab Blog"; link = "/FsLab Blog"})

    siteContent
