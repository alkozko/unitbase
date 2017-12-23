namespace Demostore.Application

type ReplicationMode = OneNode | Async

type AplicationMode = Regular | Leader | Follower

module Settings = 

    let mutable Port = "9000"
    let mutable Followers : string[] = [||]
    let mutable ReplicationMode = ReplicationMode.OneNode
    let mutable AplicationMode = AplicationMode.Regular