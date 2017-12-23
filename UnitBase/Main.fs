namespace Unitbase

module Main=
    
    open Nancy
    open Nancy.Hosting.Self
    open System
    open Microsoft.FSharp.Reflection
    open Unitbase.Application

    let baseUriString = "http://localhost:"


    let fromString<'a> (s:string) =
        match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> (case.Name.ToLowerInvariant()) = (s.ToLowerInvariant())) with
        |[|case|] -> Some(FSharpValue.MakeUnion(case,[||]) :?> 'a)
        |_ -> None


    [<EntryPoint>]
    let main argv =
        
        for arg in argv do
            match arg with
            | x when x.StartsWith "-port" -> Settings.Port <- (x.Split('=').[1])
            | x when x.StartsWith "-followers" -> Settings.Followers <- (x.Split('=').[1].Split(','))
            | x when x.StartsWith "-repication" -> Settings.ReplicationMode <- (fromString<ReplicationMode> (x.Split('=').[1])).Value
            | x when x.StartsWith "-mode" -> Settings.AplicationMode <- (fromString<AplicationMode> (x.Split('=').[1])).Value
            | _ -> ()
        
        let hostConfigs = HostConfiguration()
        hostConfigs.UrlReservations.CreateAutomatically <- true
        let nancy = new NancyHost(Uri(baseUriString + Settings.Port), new DefaultNancyBootstrapper(), hostConfigs)
        nancy.Start()
        Console.WriteLine("Nancy is running at {0}", baseUriString + Settings.Port)
        while true do Console.ReadLine() |> ignore
        0
