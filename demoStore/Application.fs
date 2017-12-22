namespace Demostore.Application

type ReplicationMode = Nothing | Async

type AplicationMode = Regular | Leader | Follower

module Settings = 

    let mutable Port = "9000"
    let mutable Followers : string[] = [||]
    let mutable ReplicationMode = ReplicationMode.Nothing
    let mutable AplicationMode = AplicationMode.Regular