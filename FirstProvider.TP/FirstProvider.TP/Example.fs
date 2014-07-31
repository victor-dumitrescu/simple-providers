module Example

open FSharp.ProvidedTypes.GeneralCombinators
open FSharp.ProvidedTypes.CloneCombinator
open FSharp.ProvidedTypes.AddStaticCombinator
open Microsoft.FSharp.Core.CompilerServices

//let Example1 = 
//    let SimpleProvider =
//        new FirstProvider.TP.MyTypeProvider()
//    Clone("FirstTypeProvider.Something", "MySpace", SimpleProvider)


let ExampleAddStatic = 
    let SimpleProvider =
        Clone("FirstTypeProvider.Something", "MySpace", new FirstProvider.TP.MyTypeProvider())
    AddStaticParameter("AnotherOne", typeof<string>, "defaultv", SimpleProvider)

//[<TypeProvider>]
//type ExampleProvider() = inherit TypeProviderExpression(Example1)

[<TypeProvider>]
type AddStaticExample() = inherit TypeProviderExpression(ExampleAddStatic)

[<assembly:TypeProviderAssembly>]
do()
    