// Replacement for /pentest/enumeration/list-urls
// TODO: make fsx friendly, take a file as list, hostname resolver, crawler, https

#light
open System.Net
open System.Text.RegularExpressions
open Microsoft.FSharp.Control.WebExtensions

let urlList = ["1","http://google.com"]

let extractLinksAsync html =
    async {
        let linkCount = Regex.Matches(html, @"http://\S+")
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
            printf "Finished, Total links: %s : %d" url links.Count

        with | ex -> printfn "%s" (ex.Message)
    }


let runAll =
    urlList
        |> Seq.map fetchAsync
        |> Async.Parallel 
        |> Async.RunSynchronously
        |> ignore

