#load "./TestingUtils.fsx"

open System
open TestingUtils


let Run extendedLogging pause master cluster = 
    let records = [1..100]
                    |> List.map (fun _ -> (Guid.NewGuid().ToString().Replace("-","")))

    let results =
        records 
            |> List.map ((fun r ->
            async {
                let! res = write (randomNode master) r
                do res.EnsureSuccessStatusCode() |> ignore
                let node = (randomNode cluster)
                if (pause) then
                    do! Async.Sleep 100
                let! readResult = read node
                if r <> readResult && extendedLogging then
                    console.writeError <| sprintf "Expected %s, received %s from %s" r readResult (fst node)

                return r = readResult
            })
            >> Async.RunSynchronously)

    let pauseappendix = (if pause then "with pauses" else "")

    if (results |> List.contains false) then
        console.writeError <| sprintf "FAILED Sequential read\\write %s; failed %i from %i" pauseappendix (results |> List.where not |> List.length) results.Length
    else
        console.writeOk <| sprintf "PASSED Sequential read\\write %s" pauseappendix
