namespace Unitbase.Distribution
open System.Net.Http
open System.Text
open Unitbase.Application
open System

type SyncReplicationLogic(mode, followers) =
    inherit DistributionBaseLogic()
    
    let replicate (value:string) url =
        async {
            use client = new HttpClient();
            client.DefaultRequestHeaders.Clear()        
            let url = sprintf "%s/replica" url
            use content = new ByteArrayContent(Encoding.UTF8.GetBytes(value))
            let! result = client.PutAsync(url, content) |> Async.AwaitTask;
            return result
        }

    override x.Read replica = 
        x.ReadData()

    override x.Write replica content =
        async {
            if (mode <> AplicationMode.Leader && not replica) then
                raise <| InvalidOperationException()

            if not replica then 
                let! responses =
                    followers
                    |> Array.map (replicate content)
                    |> Async.Parallel
                responses
                    |> Array.iter (fun t -> t.EnsureSuccessStatusCode() |> ignore)

            x.WriteData content            
        }