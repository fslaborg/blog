// This file contains functions that can be used everywhere.
// it should be loaded as the first scriptg in a new loader or generator.

#r "nuget: Fornax.Core, 0.15.1"
#r "nuget: FsLab.Fornax, 2.2.0"

open System.IO
open System.Text.RegularExpressions
open Html

// fix urls when deployed to base url (e.g. on gh pages via subdomain)
#if WATCH
let urlPrefix = 
  ""
#else
let urlPrefix = 
  "https://fslab.org/blog"
#endif

/// returns a fixed urlby prefixing `urlPrefix`
let prefixUrl url = sprintf "%s/%s" urlPrefix url

///`fileContent` - content of page to parse. Usually whole content of `.md` file
///returns content of config that should be used for the page
let getFrontmatter (fileContent : string) =
    let isSeparator (input : string) =
        input.StartsWith "---"
    let fileContent = fileContent.ReplaceLineEndings(System.Environment.NewLine) // normalize line endings
    let fileContent = fileContent.Split System.Environment.NewLine
    let fileContent = fileContent |> Array.skip 1 //First line must be ---
    let indexOfSeperator = fileContent |> Array.findIndex isSeparator
    let splitKey (line: string) =
        let seperatorIndex = line.IndexOf(':')
        if seperatorIndex > 0 then
            let key = line.[.. seperatorIndex - 1].Trim().ToLower()
            let value = line.[seperatorIndex + 1 ..].Trim()
            Some(key, value)
        else
            None
    fileContent
    |> Array.splitAt indexOfSeperator
    |> fst
    |> Seq.choose splitKey
    |> Map.ofSeq

let processConvertedNotebook (content:string) =
    let nb_start_tag = """<body class="jp-Notebook" data-jp-theme-light="true" data-jp-theme-name="JupyterLab Light">"""
    let body_start_index = content.IndexOf(nb_start_tag) + nb_start_tag.Length
    let body_end_index = content.IndexOf "</body>" + 1
    content[body_start_index .. body_end_index]

let fixNotebookJson (language:string) (nb_path:string) =
    let content = File.ReadAllText(nb_path)
    if language = "fsharp" then
        if not (content.Contains("\"language_info\": {\"name\": \"F#\"}")) then
            printfn $"fixing fsharp notebook json for {nb_path}"
            let metadataSection = "\"metadata\": {"
            let metadata_start_index = content.LastIndexOf(metadataSection)
            File.WriteAllText(nb_path, (content[0..metadata_start_index + metadataSection.Length] + "\"language_info\": {\"name\": \"F#\"}," + content[metadata_start_index + metadataSection.Length..]))
    elif language = "csharp" then
        if content.Contains("\"name\": \"polyglot-notebook\"") then
            File.WriteAllText(nb_path, (content.Replace("\"name\": \"polyglot-notebook\"","\"name\": \"C#\"")))
        elif not (content.Contains("\"language_info\": {\"name\": \"C#\"}")) then
            printfn $"fixing fsharp notebook json for {nb_path}"
            let metadataSection = "\"metadata\": {"
            let metadata_start_index = content.LastIndexOf(metadataSection)
            File.WriteAllText(nb_path, (content[0..metadata_start_index + metadataSection.Length] + "\"language_info\": {\"name\": \"C#\"}," + content[metadata_start_index + metadataSection.Length..]))

let anchorRegex = Regex("<a class=\"anchor-link\" href=\"(?<link>#\\S*)\"", RegexOptions.Compiled)

let getNotebookTOC (content:string) =
    let anchors =
        anchorRegex.Matches(content)
        |> Seq.map (fun x -> x.Groups.Item("link").Value)
        |> List.ofSeq
        
    ul [] (
        anchors
        |> List.map (fun link ->
            let title = link.Replace("-"," ").Replace("#","")
            li [] [
                a [Href link; Class "is-aquamarine"] [!!title]
            ]
        ) 
    )

/// injects websocket code necessary for hot reload on local preview via `dotnet fornax watch`
let injectWebsocketCode (webpage:string) =
    let websocketScript =
        """
        <script type="text/javascript">
          var wsUri = "ws://localhost:8080/websocket";
      function init()
      {
        websocket = new WebSocket(wsUri);
        websocket.onclose = function(evt) { onClose(evt) };
      }
      function onClose(evt)
      {
        console.log('closing');
        websocket.close();
        document.location.reload();
      }
      window.addEventListener("load", init, false);
      </script>
        """
    let head = "<head>"
    let index = webpage.IndexOf head
    webpage.Insert ( (index + head.Length + 1),websocketScript)