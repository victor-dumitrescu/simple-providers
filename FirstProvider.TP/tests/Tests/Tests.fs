module TPCombinators.Tests

type Mine = MySpace.NewType<"stuff">

printfn "%A" Mine.Hello

let thing = new Mine(42)

let thing2 = new Mine()
printfn "%A" thing2.``Internal State``


