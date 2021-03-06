﻿//From: https://skillsmatter.com/skillscasts/5138-creating-type-providers-hands-on-with-michael-newton
//End time: 1:00:00


module FirstProvider.TP

open ProviderImplementation.ProvidedTypes //provides APIs
open Microsoft.FSharp.Core.CompilerServices //for code quotations
open System.Reflection

[<TypeProvider>]     //could use a TypeProviderConfig here
type public MyTypeProvider () as this =
    //this is a class that can generate namespaces and put types in it
    inherit TypeProviderForNamespaces ()

    //Using reflection -> get the assembly of the TP
    let asm = Assembly.GetExecutingAssembly()

    //This namespace will have all the types and will be added to the TP
    let ns = "FirstTypeProvider.Something"

    let newType =                                  //erases type to obj
        ProvidedTypeDefinition(asm, ns, "NewType", Some typeof<obj>)

    do 
        newType.DefineStaticParameters([ProvidedStaticParameter("Sample", typeof<string>, parameterDefaultValue = "def")], 
                                        (fun tName args -> 
                                            let newType =
                                                ProvidedTypeDefinition(asm, ns, tName, Some typeof<obj>)

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
                                                    InvokeCode = fun _ -> <@ 3 :> obj @>.Raw
                                                    )

                                            let paramCons =
                                                ProvidedConstructor(
                                                    [ProvidedParameter("Internal state", typeof<int>)],
                                                    InvokeCode = fun argsx -> <@ (%%(argsx.[0]) : int) :> obj @>.Raw
                                                    )

                                            let internalState =
                                                ProvidedProperty(
                                                    "Internal State",
                                                    typeof<int>,
                                                    IsStatic = false,
                                                    GetterCode = (fun argsy -> <@ (%%argsy.[0] :> obj) :?> int @>.Raw)
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
                                                                                                                                
                                            newType ))

    do
        this.AddNamespace(ns, [newType])

[<assembly:TypeProviderAssembly>]
do ()


