module TPCombinators.Tests

// #r @"C:\GitHub\victor-dumitrescu\simple-providers\FirstProvider.TP\FirstProvider.TP\bin\Debug\FirstProvider.TP.dll"
type Mine = MySpace.NewType<AnotherOne="abc">
type Mine2 = MySpace.NewType<Sample="abc">
type Mine3 = MySpace.NewType<AnotherOne="def",Sample="abc">
//type Mine4 = MySpace.NewType< >


printfn "%A" Mine.Hello

let thing = new Mine(42)

let thing2 = new Mine()
printfn "%A" thing2.``Internal State``


