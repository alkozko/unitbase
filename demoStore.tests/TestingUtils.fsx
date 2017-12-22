open System.Diagnostics
open System.IO
#r @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\System.Net.Http.dll"

open System.Net.Http
open System.Text
open System
open System.Threading.Tasks
open System.Threading

module client = 
    let private getClient() =
        let client = new HttpClient();
        client.DefaultRequestHeaders.Clear() 
        client

    let private makeUrl = sprintf "%s/data" 

    let put baseUrl (value:string) =
        async {
            use client = getClient();
            use content = new ByteArrayContent(Encoding.UTF8.GetBytes(value))
            let! result = client.PutAsync(makeUrl baseUrl, content) |> Async.AwaitTask;
            return result
        }

    let get<'a> baseUrl =
        async {
            use client = getClient();
            let! result = client.GetAsync(makeUrl baseUrl) |> Async.AwaitTask;
            return! result.Content.ReadAsStringAsync() |> Async.AwaitTask;
        }

module console = 
    let writeError (text:string) =
        let color = Console.ForegroundColor 
        Console.ForegroundColor <- ConsoleColor.Red
        Console.WriteLine(text)
        Console.ForegroundColor <- color

    let writeOk (text:string) =
        let color = Console.ForegroundColor 
        Console.ForegroundColor <- ConsoleColor.Green
        Console.WriteLine(text)
        Console.ForegroundColor <- color
 

module processManagment =  
    let private startDemostore path port =
        let arguments = sprintf "-port=%i" port 
        Process.Start(path, arguments)

    let private stopProcess (proc: Process) = 
        proc.Kill()
        proc.Dispose()

    let start path port = 
        sprintf "http://localhost:%i" port, startDemostore path port

    let stop proc = stopProcess <| snd proc

let private randomGen = Random()

let getRandom max =
    randomGen.Next(max)

let randomNode cluster =
    let index = getRandom <| List.length cluster
    cluster.[index];

let startTask func =
    let tokenSource = new CancellationTokenSource()
    let ct = tokenSource.Token
    let action = (fun _ -> func(ct))
    let task = Task.Factory.StartNew(action, tokenSource)
    task, tokenSource


let path =  Path.Combine(Directory.GetParent(__SOURCE_DIRECTORY__ ).FullName, @"demostore\bin\Debug\net461\demostore.exe")

let read proc = client.get <| fst proc
let write proc = client.put <| fst proc
