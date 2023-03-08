#r "../_lib/Fornax.Core.dll"

open System.IO

// Static files are served as-is
let generate (ctx : SiteContents) (projectRoot: string) (page: string) =
    let inputPath = Path.Combine(projectRoot, page)
    File.ReadAllBytes inputPath  