# Unit Base

A single-value distributed database, written on F#

Support Read and Write operations

Support Async and Sync replication modes, SingleNode mode supported as well

## Usage 

See test-script [TestsRun.fsx](/UnitBase.Tests/TestsRun.fsx)

For run node of application need pass command-line arguments:

/unitbase.exe -port=[Number] -repication=[OneNode|Async|Sync] -mode=[Regular|Leader|Follower] -followers=[url1,url2...]


## Problems

There are some problems with consistency (linearizability). For demostrate it see [TestsRun.fsx](/UnitBase.Tests/TestsRun.fsx)

### Explanation

Async tests crach due to replication lags

Sync tests crash due to reordering replication reqests
