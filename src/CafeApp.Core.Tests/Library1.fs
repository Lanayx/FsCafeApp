namespace CafeApp.Core.Testsf
open CafeApp.Core 
open NUnit.Framework 

module UnitTests =  
    [<Test>]  
    let ``Sample Test``() =    
        let class1 = new Class1()    
        Assert.AreEqual("F#", class1.X)
