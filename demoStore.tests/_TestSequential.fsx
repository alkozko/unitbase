#load "./TestingUtils.fsx"

open System
open TestingUtils


let Run master cluster = 
    let records = [1..1000]
                    |> List.map (fun _ -> (Guid.NewGuid().ToString().Replace("-","")))

    let results =
        records 
            |> List.map ((fun r ->
            async {
                let! res = write (randomNode master) r
                do res.EnsureSuccessStatusCode() |> ignore
                let! readResult = read (randomNode cluster)
                if r <> readResult then
                    console.writeError <| sprintf "Expected %s, received %s" r readResult

                return r <> readResult
            })
            >> Async.RunSynchronously)

    if (results |> List.contains false) |> not then
        console.writeError "FAILED Sequential read\\write test"
    else
        console.writeOk "PASSED Sequential read\\write test"
