namespace Demostore.Distribution

type SingleNodeLogic() =
    inherit DistributionBaseLogic()
    
    override x.Read replica = 
        x.ReadData()

    override x.Write replica content =
        async {
            return x.WriteData content
        }