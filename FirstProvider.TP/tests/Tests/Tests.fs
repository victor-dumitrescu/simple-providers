module TPCombinators.Tests

type Mine = MySpace.NewType
let thing = new Mine("content")

let thing2 = new Mine()
printfn "%A" thing2.``Internal State``

