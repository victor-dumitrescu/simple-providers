// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "bin/Debug/FirstProvider.TP.dll"
open FirstTypeProvider

Something.NewType.Hello
type Conn = Something.NewType
let thing = new Conn("stuff")
thing.``Internal State``

let myThing = Something.NewType()

let myThing2 = Something.NewType("parameter yay")
myThing2.``Internal State``





// Define your library scripting code here

