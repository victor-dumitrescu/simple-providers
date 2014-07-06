//From: https://skillsmatter.com/skillscasts/5138-creating-type-providers-hands-on-with-michael-newton
//End time: 1:00:00


module FirstProvider.TP

open ProviderImplementation.ProvidedTypes //provides APIs
open Microsoft.FSharp.Core.CompilerServices //lookup code quotations
open System.Reflection

[<TypeProvider>]
type MyTypeProvider () as this =
    //this is a class that can generate namespaces and put types in it
    inherit TypeProviderForNamespaces ()

    let asm = Assembly.GetExecutingAssembly()
    let ns = "FirstTypeProvider.Something"

    let newType =                                  //erases type to obj
        ProvidedTypeDefinition(asm, ns, "NewType", Some typeof<obj>)

    let helloWorld =
        ProvidedProperty(
            "Hello",
            typeof<string>,
            IsStatic = true,
            GetterCode =
                (fun _ -> <@ "hello world" @>.Raw )
            )

    let cons =
        ProvidedConstructor(
            [],
            InvokeCode = fun _ -> <@ "My internal state" :> obj @>.Raw
            )

    let paramCons =
        ProvidedConstructor(
            [ProvidedParameter("Internal state", typeof<string>)],
            InvokeCode = fun args -> <@ (%%(args.[0]) : string) :> obj @>.Raw
            )

    let internalState =
        ProvidedProperty(
            "Internal State",
            typeof<string>,
            IsStatic = false,
            GetterCode = (fun args -> <@ (%%args.[0] :> obj) :?> string @>.Raw)
            )

    let prefixState =
        ProvidedMethod(
            "PrefixState",
            [ProvidedParameter("Prefix", typeof<string>)],
            typeof<string>,
            IsStaticMethod = false,
            InvokeCode = 
                fun [self; prefix] ->
                    <@ (%%prefix : string) + (%%self :> obj :?> string) @>.Raw
        )

    do 
        newType.AddMember helloWorld
        newType.AddMember cons //Note: you can also add subtypes with AddMember
        newType.AddMember paramCons
        newType.AddMember internalState
        newType.AddMember prefixState

    do
        this.AddNamespace(ns, [newType])

[<assembly:TypeProviderAssembly>]
do ()


