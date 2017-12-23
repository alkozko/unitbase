#load "./TestingUtils.fsx"

open System
open TestingUtils


let Run pause master cluster = 

    master 
    |> List.iter (fun n -> write n "value" |> Async.RunSynchronously |> ignore)

    if (pause) then
        Async.Sleep 100 |> Async.RunSynchronously

    let results =
        cluster
        |> List.map (read >> Async.RunSynchronously >> (fun _val -> _val = "value"))

    let pauseappendix = (if pause then "with pauses" else "")

    if (results |> List.contains false) then
        console.writeError <| sprintf "FAILED Master-slave replication %s; failed %i from %i" pauseappendix (results |> List.where not |> List.length) results.Length
    else
        console.writeOk <| sprintf "PASSED Master-slave replication %s" pauseappendix
