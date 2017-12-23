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
let amaster, acluster = processManagment.createCluser "async" 3

_TestMasterSlave.Run true amaster acluster
_TestSequential.Run false false amaster acluster // FAIL
_TestSequential.Run false true amaster acluster
_TestParallelReadWrite.Run false amaster acluster // FAIL
_TestParallelReadWrite.Run true amaster acluster

processManagment.stopCluster acluster
console.writeOk "====== Async Tests FINISHED ===="


console.writeOk "====== Sync Tests STARTED===="
let smaster, scluster = processManagment.createCluser "sync" 3

_TestMasterSlave.Run true smaster scluster
_TestSequential.Run false false smaster scluster
_TestParallelReadWrite.Run false smaster scluster 

processManagment.stopCluster scluster
console.writeOk "====== Sync Tests FINISHED ===="
