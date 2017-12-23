#load "./TestingUtils.fsx"

open System
open TestingUtils

let toPair fst snd =
        (fst, snd)

let private writeErrors (results:(string*string)[]) =
    let resultsRepresentation =
        results 
        |> Array.map (fun r -> String.Join(" - ", [| fst r; snd r|]))
        |> toPair "\r\n"
        |> String.Join

    console.writeError "ERROR"
    console.writeError resultsRepresentation
    console.writeError ""
    console.writeError ""

let Run extendedLogging master cluster = 
    let records = [1..1000]
                |> List.map (fun _ -> (Guid.NewGuid().ToString().Replace("-","")))

    let paged pageSize records =
        [0..pageSize..(List.length records)]
        |> List.take (records.Length / pageSize)
        |> List.map (fun i -> records.[i..(i+(pageSize-1))])

    let result = write (master |> List.head) "" |> Async.RunSynchronously
    result.EnsureSuccessStatusCode()

    let batches = paged 3 records
    
    let results =
        batches 
        |> List.map (fun recs ->
                let res = recs 
                            |> List.map (fun record -> write master.Head record) 
                            |> Async.Parallel
                            |> Async.RunSynchronously

                let wrResults = cluster 
                                |> List.map (fun n -> async{
                                        let! result = read n
                                        return fst n, result
                                    })
                                |> Async.Parallel
                                |> Async.RunSynchronously
                
                if wrResults |> Array.groupBy snd |> Array.length > 1 then
                    if extendedLogging then
                        writeErrors wrResults
                    false
                else
                    true
        )
        
    if (results |> List.contains false) then
        console.writeError <| sprintf "FAILED Parallel write; failed %i from %i" (results |> List.where not |> List.length) results.Length
    else
        console.writeOk "PASSED Parallel write"