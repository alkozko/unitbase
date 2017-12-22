namespace Demostore

module Main=
    
    open Nancy
    open Nancy.Hosting.Self
    open System
    open Demostore.Application

    let baseUriString = "http://localhost:"

    [<EntryPoint>]
    let main argv =
        
        for arg in argv do
            match arg with
            | x when x.StartsWith "-port" -> ApplicationSettings.Port <- (x.Split('=').[1])
            | _ -> ()
        
        let hostConfigs = HostConfiguration()
        hostConfigs.UrlReservations.CreateAutomatically <- true
        let nancy = new NancyHost(Uri(baseUriString + ApplicationSettings.Port), new DefaultNancyBootstrapper(), hostConfigs)
        nancy.Start()
        Console.WriteLine("Nancy is running at {0}", baseUriString + ApplicationSettings.Port)
        while true do Console.ReadLine() |> ignore
        0
