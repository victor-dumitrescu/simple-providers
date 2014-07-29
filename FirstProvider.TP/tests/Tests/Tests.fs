module TPCombinators.Tests

// #r @"C:\GitHub\victor-dumitrescu\simple-providers\FirstProvider.TP\FirstProvider.TP\bin\Debug\FirstProvider.TP.dll"
type Mine = MySpace.NewType<"stuff">

printfn "%A" Mine.Hello

let thing = new Mine(42)

let thing2 = new Mine()
printfn "%A" thing2.``Internal State``


