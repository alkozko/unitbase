open System.Diagnostics
open System.Collections.Generic
#load "./TestingUtils.fsx"

open System
open TestingUtils


let Run pause master cluster = 
    let records = [1..100]
                |> List.map (fun _ -> (Guid.NewGuid().ToString().Replace("-","")))
    let result = write (master |> List.head) "" |> Async.RunSynchronously
    result.EnsureSuccessStatusCode()
    let readTimes = List<string*(int64*int64)>()

    let timer = Stopwatch.StartNew();

    let task, tokenSource = 
        startTask (fun ct -> 
            while true do
                ct.ThrowIfCancellationRequested();

                let node = (randomNode cluster)
                let start = timer.ElapsedMilliseconds;
                let res = read node |> Async.RunSynchronously
                let stop = timer.ElapsedMilliseconds;
                readTimes.Add(res, (start, stop))
        )

            
    let writeTimes = 
        String.Empty::records
        |> List.map (fun str ->
            if String.Empty = str then
                (str, (0L, 0L))
            else 
                let start = timer.ElapsedMilliseconds;
                let res = write (randomNode master) str |> Async.RunSynchronously
                if (pause) then
                    Async.Sleep 100 |> Async.RunSynchronously
                let stop = timer.ElapsedMilliseconds;
                res.EnsureSuccessStatusCode() |> ignore
                (str, (start, stop))
        )

    tokenSource.Cancel();
    timer.Stop()

    let writeTable = 
        writeTimes
        |> List.windowed 2
        |> List.map (fun p -> (fst p.[0], (snd p.[0], snd p.[1])))
        |> List.append ([writeTimes |> List.map (fun (str, time) -> (str, ((time),(Int64.MaxValue,Int64.MaxValue)))) |> List.last])
        |> dict

    let readResults = 
        readTimes
        |> Seq.toList
        |> List.rev
        |> List.map (fun (str, (start, stop)) ->
            let ((wrstart,_),(_, rrstop)) = writeTable.[str]
            let overlap = wrstart < stop && start < rrstop
            overlap
        )
        |> List.countBy id

    let pauseappendix = (if pause then "with pauses" else "")
    let falseResults = readResults |> List.where (fst >> not)

    if (falseResults |> List.isEmpty |> not) then
        console.writeError <| sprintf "FAILED Parallel Read\\Write %s; failed %i from %i" pauseappendix (falseResults |> List.head |> snd) readTimes.Count
    else
        console.writeOk <| sprintf "PASSED Parallel Read\\Write replication %s" pauseappendix

