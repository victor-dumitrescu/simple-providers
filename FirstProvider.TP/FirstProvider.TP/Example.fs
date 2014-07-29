module Example

open FSharp.ProvidedTypes.GeneralCombinators
open FSharp.ProvidedTypes.CloneCombinator
open Microsoft.FSharp.Core.CompilerServices

let Example1 = 
    
    let SimpleProvider =
//        let FSharpDataAssembly = typeof<MyTypeProvider>.Assembly
        new FirstProvider.TP.MyTypeProvider()

    Clone("FirstTypeProvider.Something", "MySpace", SimpleProvider)

[<TypeProvider>]
type ExampleProvider() = inherit TypeProviderExpression(Example1)

[<assembly:TypeProviderAssembly>]
do()
