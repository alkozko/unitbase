namespace  Demostore

open Nancy
open System.IO
open Demostore.Distribution
open Demostore.Application

module App =
    
    let (?) (object : 'Source) (prop : string) : 'Result =
        let value = (object :> obj :?> DynamicDictionary).[prop] :?> DynamicDictionaryValue
        value.Value :?> 'Result


    let getDistributionLogic() =
        match Settings.ReplicationMode with
        | Nothing -> SingleNodeLogic() :> DistributionBaseLogic
        | Async -> AsyncReplicationLogic(Settings.AplicationMode, Settings.Followers) :> DistributionBaseLogic

    type App() as this =
        inherit NancyModule()

        let getContent (request: Request) = 
            use reader = new StreamReader(request.Body)
            reader.ReadToEndAsync() |> Async.AwaitTask

        let store = getDistributionLogic()

        do this.Get.["/data", true] <- fun ctx ct ->
            async {
                return store.Read false :> obj
            }
            |> Async.StartAsTask

        do this.Put.["/data", true] <- fun ctx ct ->
            async {
                let! content = getContent this.Request
               
                try
                    do! store.Write false content
                    return HttpStatusCode.OK :> obj
                with 
                | :? System.InvalidOperationException -> return HttpStatusCode.NotImplemented :> obj
                | _ -> return HttpStatusCode.InternalServerError :> obj
            }
            |> Async.StartAsTask

        
        do this.Put.["/replica", true] <- fun ctx ct ->
            async {
                let! content = getContent this.Request
                
                do! store.Write true content

                return HttpStatusCode.OK :> obj
            }
            |> Async.StartAsTask