namespace Demostore.Distribution

type OneNodeStoreLogic() =
    inherit DistributionStoreBaseLogic()
    
    override x.Read() = 
        x.ReadData()

    override x.Write content =
        async {
            return x.WriteData content
        }