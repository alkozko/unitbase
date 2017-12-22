#load "./TestingUtils.fsx"

open System.IO
open System
open TestingUtils

let path =  Path.Combine(Directory.GetParent(__SOURCE_DIRECTORY__ ).FullName, @"demostore\bin\Debug\net461\demostore.exe")

let read proc = client.get <| fst proc
let write proc = client.put <| fst proc


let records = [1..1000]
                |> List.map (fun _ -> (Guid.NewGuid().ToString().Replace("-","")))


let n1 = processManagment.start path 9000

let results =
    records 
        |> List.map ((fun r ->
        async {
            let! res = write n1 r
            do res.EnsureSuccessStatusCode() |> ignore
            let! readResult = read n1
            if r <> readResult then
                console.writeError <| sprintf "Expected %s, received %s" r readResult

            return r <> readResult
        })
        >> Async.RunSynchronously)

processManagment.stop n1

if (results |> List.contains false) |> not then
    console.writeError "FAILED Sequential read\\write test"
else
    console.writeOk "PASSED Sequential read\\write test"