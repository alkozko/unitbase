namespace  Demostore

open Nancy
open System.IO
open Demostore.Distribution

module App =
    
    let (?) (object : 'Source) (prop : string) : 'Result =
        let value = (object :> obj :?> DynamicDictionary).[prop] :?> DynamicDictionaryValue
        value.Value :?> 'Result


    type App() as this =
        inherit NancyModule()

        let store = OneNodeStoreLogic()

        do this.Get.["/data", true] <- fun ctx ct ->
            async {
                return store.Read() :> obj
            }
            |> Async.StartAsTask



        do this.Put.["/data", true] <- fun ctx ct ->
            async {
                use reader = new StreamReader(this.Request.Body)
                let! content = reader.ReadToEndAsync() |> Async.AwaitTask
               
                do! store.Write content
               
                return HttpStatusCode.OK :> obj
            }
            |> Async.StartAsTask