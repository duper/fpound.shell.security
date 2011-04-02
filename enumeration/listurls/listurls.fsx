(* Replacement for /pentest/enumeration/list-urls
 * TODO: make fsx friendly, take a file as list, hostname resolver, crawler. *)

#light

open System
open System.Net
open System.Text.RegularExpressions
open Microsoft.FSharp.Control.WebExtensions

let args = Environment.GetCommandLineArgs()

let extractLinksAsync html =
    async {
        let linkCount = Regex.Matches(html, @"https?://\S+")
        linkCount |> Seq.cast |> Seq.iter (fun s -> printfn "\x22%A" s)
        return linkCount
    }
           
let fetchAsync(name, url:string) =
    async {
        try
            let uri = new System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri)
            let! links = extractLinksAsync html
            printfn "\n[+] Finished: %s \n[+] Total links: %d" url links.Count

        with | ex -> printfn "%s" (ex.Message)
    }

let runFetch(urlList) =
    urlList
        |> Seq.map fetchAsync
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> ignore

let geArgs() =
    let urlList = ["1", args.[1].ToString()]
    runFetch(urlList)
        
let heMenu() =
    printfn "\nList-Url 0.1\n"
    printfn "Syntax: %s [site]" args.[0]
    printfn "E.g %s http://www.google.com > output.txt" args.[0]

(* Start *)
if args.Length <= 1 then heMenu()
else geArgs()
   
