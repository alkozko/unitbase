namespace Unitbase.Distribution
open System.Threading

[<AbstractClass>]
type DistributionBaseLogic() =

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

        
    abstract member Write: bool -> string -> Async<unit>
    abstract member Read: bool -> string