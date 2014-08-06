module Example

open FSharp.ProvidedTypes.GeneralCombinators
open FSharp.ProvidedTypes.CloneCombinatorOverSimplifiedAlgebra
open FSharp.ProvidedTypes.AddStaticOverSimplifiedAlgebra
open Microsoft.FSharp.Core.CompilerServices
open FSharp.ProvidedTypes.SimplifiedAlgebra

let SimpleProvider =
    new FirstProvider.TP.MyTypeProvider() |> Simplify


let ExampleAddStatic = 
    SimpleProvider
    |> AddStaticParam("AnotherOne", typeof<string>, Some("thing":>obj))
    |> Clone("FirstTypeProvider.Something", "MySpace")
    
[<TypeProvider>]
type AddStaticExample() = inherit TypeProviderExpression(ExampleAddStatic |> Desimplify)

[<assembly:TypeProviderAssembly>]
do()
    