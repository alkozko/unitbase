#load "./TestingUtils.fsx"
#load "./_TestSequential.fsx"

open TestingUtils

console.writeOk "====== One node Tests STARTED===="
let n1 = processManagment.startOneNode 9000

_TestSequential.Run [n1] [n1]

processManagment.stop n1
console.writeOk "====== One node Tests FINISHED ===="


//let master, cluster = processManagment.createAsyncCluser 3