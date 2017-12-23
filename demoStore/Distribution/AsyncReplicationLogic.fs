namespace Demostore.Distribution
open System.Net.Http
open System.Text
open Demostore.Application
open System

type AsyncReplicationLogic(mode, followers) =
    inherit DistributionBaseLogic()
    
    let replicate (value:string) url =
        async {
            use client = new HttpClient();
            client.DefaultRequestHeaders.Clear()        
            let url = sprintf "%s/replica" url
            use content = new ByteArrayContent(Encoding.UTF8.GetBytes(value))
            let! result = client.PutAsync(url, content) |> Async.AwaitTask;
            return result |> ignore
        }

    override x.Read replica = 
        x.ReadData()

    override x.Write replica content =
        async {
            if (mode <> AplicationMode.Leader && not replica) then
                raise <| InvalidOperationException()

            x.WriteData content

            if not replica then 
                followers
                |> Array.map ((replicate content) >> Async.Start)
                |> ignore
        }