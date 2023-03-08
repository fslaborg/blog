#r "../_lib/Fornax.Core.dll"

/// Contains gloabl metadata
type SiteInfo = {
    title: string
    description: string
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add({title = "FsLab Blog"; description = "The FsLab blog"})

    siteContent