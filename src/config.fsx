#r "_lib/Fornax.Core.dll"

open Config
open System.IO

let postPredicate (projectRoot: string, page: string) =
    let fileName = Path.Combine(projectRoot,page)
    let ext = Path.GetExtension page
    if ext = ".md" then
        let ctn = File.ReadAllText fileName
        page.Contains("_public") |> not
        && ctn.Contains("layout: post")
    else
        false

let staticPredicate (projectRoot: string, page: string) =
    let ext = Path.GetExtension page
    let fileShouldBeExcluded =
        ext = ".fsx" ||
        ext = ".md"  ||
        page.Contains "_public" ||
        page.Contains "_bin" ||
        page.Contains "_lib" ||
        page.Contains "_data" ||
        page.Contains "_settings" ||
        page.Contains "_config.yml" ||
        page.Contains ".sass-cache" ||
        page.Contains ".git" ||
        page.Contains ".ionide" ||
        page.Contains ".ipynb" // we'll handle ipynb serve manually to fix paths
    fileShouldBeExcluded |> not


let config = {
    Generators = [
        {Script = "graphgallery.fsx"; Trigger = Once; OutputFile = NewFileName "graph-gallery.html"}
        {Script = "graphgalleryposts.fsx"; Trigger = Once; OutputFile = MultipleFiles id}
        {Script = "graphgallerycategories.fsx"; Trigger = Once; OutputFile = MultipleFiles id}
        {Script = "index.fsx"; Trigger = Once; OutputFile = NewFileName "index.html"}
        {Script = "posts.fsx"; Trigger = Once; OutputFile = MultipleFiles id}
        {Script = "postcategories.fsx"; Trigger = Once; OutputFile = MultipleFiles id}
        {Script = "notebooks.fsx"; Trigger = Once; OutputFile = MultipleFiles id}
        {Script = "staticfile.fsx"; Trigger = OnFilePredicate staticPredicate; OutputFile = SameFileName }
    ]
}
