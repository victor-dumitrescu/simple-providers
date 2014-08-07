module TPCombinators.Tests

// #r @"C:\GitHub\victor-dumitrescu\simple-providers\FirstProvider.TP\FirstProvider.TP\bin\Debug\FirstProvider.TP.dll"

type Mine = MySpace.NewType<"abc">
type Mine2 = MySpace.NewType<Sample="abc" , AnotherOne = "aa">
type Original = FirstTypeProvider.Something.NewType<Sample="aaa">
//type Mine3 = MySpace.NewType<AnotherOne="def",Sample="abc">

//Ideally
//type Mine4 = MySpace.NewType< >

printfn "%A" Mine2.Hello
printfn "%A" Original.Hello

let thing = new Mine2(42)
let thing' = new Original(32)

let thing2 = new Mine2()
let thing2' = new Original()

printfn "%A" thing.``Internal State``
printfn "%A" thing'.``Internal State``

