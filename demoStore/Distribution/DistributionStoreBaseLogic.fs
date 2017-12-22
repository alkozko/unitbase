namespace Demostore.Distribution
open System.Threading

[<AbstractClass>]
type DistributionStoreBaseLogic() =

    static let mutable data = ""
    static let locker = new ReaderWriterLockSlim()

    member internal _x.WriteData(content) =
        locker.EnterWriteLock()
        data <- content
        locker.ExitWriteLock()

    member internal _x.ReadData() =
        locker.EnterReadLock()
        let content = data
        locker.ExitReadLock()
        content

        
    abstract member Write: string -> Async<unit>
    abstract member Read: unit -> string