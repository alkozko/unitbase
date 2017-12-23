#load "./TestingUtils.fsx"
#load "./_TestSequential.fsx"
#load "./_TestMasterSlave.fsx"
#load "./_TestParallelReadWrite.fsx"

open TestingUtils

console.writeOk "====== One node Tests STARTED===="
let n1 = processManagment.startOneNode 9000

_TestSequential.Run false false [n1] [n1]

processManagment.stop n1
console.writeOk "====== One node Tests FINISHED ===="

console.writeOk ""

console.writeOk "====== Async Tests STARTED===="
let master, cluster = processManagment.createAsyncCluser 3

_TestMasterSlave.Run true master cluster
_TestSequential.Run false false master cluster // FAIL
_TestSequential.Run false true master cluster
_TestParallelReadWrite.Run false master cluster // FAIL
_TestParallelReadWrite.Run true master cluster

processManagment.stopCluster cluster
console.writeOk "====== Async Tests FINISHED ===="
