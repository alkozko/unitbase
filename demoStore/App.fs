namespace  Demostore

open Nancy
open System.IO
open System.Threading

module App =
    
    let (?) (object : 'Source) (prop : string) : 'Result =
        let value = (object :> obj :?> DynamicDictionary).[prop] :?> DynamicDictionaryValue
        value.Value :?> 'Result


    type App() as this =
        inherit NancyModule()

        static let mutable data = ""
        static let locker = new ReaderWriterLockSlim()

        do this.Get.["/data", true] <- fun ctx ct ->
            async {
                
                locker.EnterReadLock()
                let response = data :> obj
                locker.ExitReadLock()
                
                return response
            }
            |> Async.StartAsTask



        do this.Put.["/data", true] <- fun ctx ct ->
            async {
                use reader = new StreamReader(this.Request.Body)
                let! content = reader.ReadToEndAsync() |> Async.AwaitTask
                
                locker.EnterWriteLock()
                data <- content
                locker.ExitWriteLock()

                return "OK" :> obj
            }
            |> Async.StartAsTask