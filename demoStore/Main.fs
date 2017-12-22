namespace Demostore

module Main=
    
    open Nancy
    open Nancy.Hosting.Self
    open System

    let baseUriString = "http://localhost:"

    [<EntryPoint>]
    let main argv =
        
        let hostConfigs = HostConfiguration()
        hostConfigs.UrlReservations.CreateAutomatically <- true
        let nancy = new NancyHost(Uri(baseUriString + "9000"), new DefaultNancyBootstrapper(), hostConfigs)
        nancy.Start()
        Console.WriteLine("Nancy is running at {0}", baseUriString + "9000")
        while true do Console.ReadLine() |> ignore
        0
