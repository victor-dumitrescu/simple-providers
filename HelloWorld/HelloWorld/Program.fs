// Mega Hello World
//
// Take two command line arguments and then print
// them along with the current time to the console.

open System
open System.IO
open System.Text.RegularExpressions
open System.Diagnostics
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open System.Collections.Generic
open System.Windows.Forms


type Suit =
    | Heart
    | Diamond
    | Spade
    | Club

//Associating data to each union case
type PlayingCard = 
    | Ace of Suit
    | King of Suit
    | Queen of Suit
    | Jack of Suit
    | ValueCard of int * Suit

    // Adding methods and properties to a discriminated union
    member this.Value = 
        match this with
        | Ace(_) -> 11
        | King(_) | Queen (_) | Jack (_) -> 10
        | ValueCard (x, _) when x <=10 && x >= 2
            -> x
        | ValueCard(_) -> failwith "Card has invalid value."


//Simple syntax of a programming language
//Program statements
type Statement =
    | Print of string
    | Sequence of Statement * Statement
    | IfStmt of Expression * Statement * Statement

//Program expressions
and Expression = 
    | Integer of int
    | LessThan of Expression * Expression
    | GreaterThan of Expression * Expression

let program = 
    IfStmt(
        GreaterThan(
            Integer(3),
            Integer(1)),
        Print ("3 is greater than 1"),
        Sequence(
            Print("3 is not"),
            Print(" greater than 1")
            )
        )

//Binary tree using discriminated unions
type BinaryTree = 
    | Node of int * BinaryTree * BinaryTree
    | Empty

let rec printInOrder tree = 
    match tree with
    | Node (data, left, right)
        -> printInOrder left
           printfn "Node %d" data
           printInOrder right
    | Empty
        -> ()

let binTree = 
    Node(2, 
         Node(1, Empty, Empty),
         Node(4,
            Node(3, Empty, Empty),
            Node(5, Empty, Empty)
        )
    )

//Records
type Person = {First : string; Last : string; Age : int}


//Lazy evaluation
//1. Lazy types
let x = Lazy<int>.Create(fun () -> printfn "Evaluating x..."; 10)
let y = lazy (printfn "Evaluating y..."; x.Value + x.Value)


//2. Sequences
let sequenceOfNumbers = seq {1 .. 5}
sequenceOfNumbers |> Seq.iter (printfn "%d")

//The memory effect of lists vs. sequences
let allPositiveIntsSeq = 
    seq {for i in 1 .. System.Int32.MaxValue do
                yield i}
    // A similar list would throw a memory exception.

// sequence expressions <=> list comprehension
let alphabet = seq {for c in 'A' .. 'Z' -> c}
Seq.takeWhile (fun x -> x < 'D') alphabet |> ignore

let noisyAlphabet = 
    seq {
        for c in 'A' .. 'Z' do
            printfn "Yielding %c..." c
            yield c
    }

//Recursive sequence for listing all files under a folder
let rec allFilesUnder basePath = 
    seq {
        //Yield all files in base folder
        yield! Directory.GetFiles(basePath)
        
        //Yield all files in its subfolders
        for subdir in Directory.GetDirectories(basePath) do
            yield! allFilesUnder subdir
    }


//Sequence module functions
// Seq.unfold -- recursive Fibonacci
let nextFibUnder100 (a, b) = 
    if a+b > 100 then
        None
    else
        let nextValue = a + b
        Some(nextValue, (nextValue, a))

let fibsUnder100 = Seq.unfold nextFibUnder100 (0, 1)
Seq.toList fibsUnder100 |> ignore

//Query expressions - super important
let activeProcCount = 
    query {
        for activeProc in Process.GetProcesses() do
            count
    }

let memoryHog = 
    query {
        for activeProcess in Process.GetProcesses() do
            sortByDescending activeProcess.WorkingSet64
            head
    }

printfn "'%s' has a working set of:\n%d bytes"
        memoryHog.MainWindowTitle memoryHog.WorkingSet64

//Query operators
//1. Selection
let windowedProcesses = 
    query {
        for activeProcess in Process.GetProcesses() do
        where (activeProcess.MainWindowHandle <> nativeint 0)
        select activeProcess }

let printProcessList procSeq= Seq.iter (printfn "%A") procSeq

let isChromeRunning = 
    query {
        for windowedProc in windowedProcesses do
        select windowedProc.ProcessName
        contains "chrome" }

let numOfServiceProcesses = 
    query {
        for activeProcess in Process.GetProcesses() do
        where (activeProcess.MainWindowHandle = nativeint 0)
        select activeProcess
        count }

//distinct
let oneHundredNos = 
    let rng = new System.Random()
    seq {
        for i = 1 to 100 do
            yield rng.Next() % 50 }

let distinctNumbers = 
    query {
        for randomNo in oneHundredNos do
        select randomNo
        distinct }

printfn "%d distinct numbers found." <| Seq.length distinctNumbers

(*
Chapter 4. Imperative Programming
*)

//Changing values (mutable)
let mutable message = "World"
printfn "Hello, %s" message
message <- "Universe"
printfn "Hello, %s" message

//Reference cells (store mutable data on heap, rather than stack)
let planets =
    ref ["Mercury"; "Venus"; "Earth"; 
         "Mars"; "Jupiter"; "Saturn"; 
         "Uranus"; "Neptune"; "Pluto" ]

planets := !planets |> List.filter (fun x -> x <> "Pluto")

//Mutable Record Types
type MutableCar = {Make : string; Model : string; mutable Miles : int}

let driveForASeason car = 
    let rng = new Random()
    car.Miles <- car.Miles + rng.Next() % 1000

//Units of measure
[<Measure>]
type far =
    static member ConvertToCel(x : float<far>) =
        (5.0<cel> / 9.0<far>) * (x - 32.0<far>)
and [<Measure>] cel = 
    static member ConvertToFar(x: float<cel>) = 
        (9.0<far> / 5.0<cel> * x) + 32.0<far>  

printfn "%A" (far.ConvertToCel(100.0<far>))

//Converting between units of measure
[<Measure>]
type rads

let halfPi = System.Math.PI * 0.5<rads>
printfn "%F" (sin (float halfPi))

//Generic units of measure
type Point< [<Measure>] 'u > (x : float<'u>, y : float<'u>) =

    member this.X = x
    member this.Y = y

    member this.UnitlessX = float x
    member this.UnitlessY = float y

    member this.Length =
        let sqr x = x * x
        sqrt <| sqr this.X + sqr this.Y

    override this.ToString() =
        sprintf
            "{%f, %f}"
            this.UnitlessX
            this.UnitlessY
    
let p = new Point<m>(10.0<m>, 10.0<m>)

//Arrays
//using array comprehension
let perfectSquares = [| for i in 1 .. 7 -> i * i |]

//encrypting a letter using ROT13
let rot13Encrypt (letter : char) = 
    if Char.IsLetter(letter) then
        let newLetter =
            (int letter)
            |> (fun letterIdx -> letterIdx - (int 'A'))
            |> (fun letterIdx -> (letterIdx + 13) % 26)
            |> (fun letterIdx -> letterIdx + (int 'A'))
            |> char
        newLetter
    else
        letter

let encryptText (text : char[]) =
    for idx = 0 to text.Length - 1 do
        let letter = text.[idx]
        text.[idx] <- rot13Encrypt letter

//creating arrays - initialize array of sine-wave elements
let divisions = 4.0
let twoPi = 2.0 * Math.PI

let wave = Array.init (int divisions) (fun i -> float i * twoPi / divisions)

let emptyIntArray : int[] = Array.zeroCreate 3
let emptyStringArray : string[] = Array.zeroCreate 3 //default - null

//mutable collection types - List
let planets' = new List<string>()
planets'.Add("Mercury")
planets'.Add("Venus")
planets'.Add("Earth")
planets'.Add("Mars")
planets'.Count |> ignore
planets'.AddRange( [| "Jupiter"; "Pluto"|])
planets'.Remove("Pluto") |> ignore


(*
Chapter 5: OOP
*)

//Explicit class construction
type OOPoint =
    val m_x : float
    val m_y : float

    //Constructor 1 - takes 2 parameters
    new (x, y) = {m_x = x; m_y = y}

    //Constructor 2 - takes no params
    new () = {m_x = 0.0; m_y = 0.0}

    member this.Length = 
        let sqr x = x * x
        sqrt <| sqr this.m_x + this.m_y

let p1 = new OOPoint(1.0, 1.0)
let p2 = new OOPoint()

//Implicit class construction
type Point2(x : float, y : float) =
    
    let length =
        let sqr x = x * x
        sqrt <| sqr x + sqr y
    do printfn "Initialized to [%f, %f]" x y

    member this.X = x
    member this.Y = y
    member this.Length = length

    //Define custom constructors (calling the 'main' one)
    new() = new Point2(0.0, 0.0)

    new(text : string) =
        if text = null then
            raise <| new ArgumentException("text")

        let parts = text.Split([| ',' |])
        let (successX, x) = Double.TryParse(parts.[0])
        let (successY, y) = Double.TryParse(parts.[1])

        if not successX || not successY then
            raise <| new ArgumentException("text")
        new Point2(x, y)

let p3 = Point2("3.0, 4.0")

//Generic classes
type Arrayify<'a>(x : 'a) =
    member this.EmptyArray : 'a[] = [| |]
    member this.Array1 : 'a[] = [| x |]
    member this.Array2 : 'a[] = [| x; x |]

let ArrayifyTuple = new Arrayify<int * int>( (10, 28) )

//Properties - setters, getters
//E.g. define a waterbottle type w/ 2 props
[<Measure>]
type ml

type WaterBottle() =
    let mutable m_amount = 0.0<ml>

    //read-only property
    member this.Empty = (m_amount = 0.0<ml>)

    //read-write property
    member this.Amount with get() = m_amount
                       and  set newAmt = m_amount <- newAmt

let bottle = new WaterBottle()

//example above rewritten with auto-properties
type WaterBottle2() =
    member this.Empty = (this.Amount = 0.0<ml>)
    member val Amount = 0.0<ml> with get, set

let bottle2 = new WaterBottle2()

//setting properties in the constructor
//1. the hard way
let f1 = new Form()
f1.Text <- "Window Title"
f1.TopMost <- true
f1.Width <- 640
f1.Height <- 480
f1.ShowDialog() 

//2. the easy way
let f2 = new Form(Text = "Window Title",
                  TopMost = true,
                  Height = 480,
                  Width = 640)
f2.ShowDialog()

//static methods, properties & fields
//1. static method in class
type SomeClass() =
    static member StaticMember() = 5

//2. static fields
type RareType() =
    
    static let mutable m_numleft = 2

    do
        if m_numleft <= 0 then
            failwith "No more left"
        m_numleft <- m_numleft - 1
        printfn "Initialized rare type, only %d left." m_numleft

    static member NumLeft = m_numleft

//Polymorphism - static upcast
[<AbstractClass>]
type Animal() =
    abstract member Legs : int

[<AbstractClass>]
type Dog() =
    inherit Animal()
    abstract member Description : string
    override this.Legs = 4

type Pomeranian() =
    inherit Dog()
    override this.Description = "furry"

let steve = new Pomeranian()
let steveAsDog = steve :> Dog
let steveAsAnimal = steve :> Animal
let steveAsObject = steve :> obj

//Polymorphism - dynamic casting (downwards)
let steveAsDog2 = steveAsObject :?> Dog

//Type testing - pattern matching
let whatIs (x : obj) =
    match x with
    | :? string as s -> printfn "x is a string \"%s\"" s
    | :? int as i -> printfn "x is an int %d" i
    | :? list<int> as l -> printfn "x is a list '%A'" l
    | _ -> printfn "x is a '%s'" <| x.GetType().Name


(*
Chapter 6: .NET Programmign
*)
//Interfaces
type Tastiness =
    | Delicious
    | SoSo
    | Not

type IConsumable =
    abstract Eat : unit -> unit
    abstract Tastiness : Tastiness

type Apple() =
    interface IConsumable with
        member this.Eat() = printfn "tasty!"
        member this.Tastiness = Delicious

let apple = new Apple()












[<Measure>]
type amu

type Atom = {Name : string; Weight : float<amu> }

let periodicTable = new Dictionary<string, Atom> ()
periodicTable.Add("H", { Name = "Hydrogen"; Weight = 1.0079<amu> })
periodicTable.Add("He", { Name = "Helium"; Weight = 4.0026<amu> }) 
periodicTable.Add("Li", { Name = "Lithium"; Weight = 6.9410<amu> }) 
periodicTable.Add("Be", { Name = "Beryllium"; Weight = 9.0122<amu> }) 
periodicTable.Add( "B", { Name = "Boron "; Weight = 10.811<amu> })

let printElement name = 
    if periodicTable.ContainsKey(name) then
        let atom = periodicTable.[name]
        printfn
            "Atom with symbol %s has weight %A."
            atom.Name atom.Weight
        else
            printfn "%s not found" name

let printElement2 name =
    let (found, atom) = periodicTable.TryGetValue(name)
    if found then
        printfn
            "Atom with symbol %s has weight %A."
            atom.Name atom.Weight
        else
            printfn "%s not found" name

//mutable collection types - HashSets
let bestPicture = new HashSet<string>()
bestPicture.Add("The Artist") |> ignore
bestPicture.Add("The King's Speech") |> ignore

if bestPicture.Contains("Trainspotting") then
    printfn "Score"

//Looping constructs - for loops with pattern matching
type Pet =
    | Cat of string * int // Name, Lives
    | Dog of string

let famousPets = [ Dog("Lassie"); Cat("Felix", 9); Dog("Rin Tin Tin") ]
for Dog(name) in famousPets do
    printfn "%s is a famous dog." name
for Cat(name, 9) in famousPets do
    printfn "%s has 9 lives." name





[<Literal>]
let Bill = "Bill Gates"

//Pattern matching - when
let highLowGame () = 
        
    let rng = new Random()
    let secretNumber = rng.Next() % 100

    let rec highLowGameStep () = 

        printfn "Guess the secret number:"
        let guess = Console.ReadLine() |> Int32.Parse

        match guess with
        | _ when guess > secretNumber
            -> printfn "The secret number is lower."
               highLowGameStep()
        | _ when guess = secretNumber
            -> printfn "Yaaas!"
               ()
        | _ when guess < secretNumber
            -> printfn "The secret number is higher."
               highLowGameStep()

    //Begin game
    highLowGameStep()

[<EntryPoint>]
let main (args : string[]) = 

    if args.Length <> 2 then

        failwith "Error: Expected arguments <greeting> and <thing>"

    let greeting, thing = args.[0], args.[1]
    let timeOfDay = DateTime.Now.ToString("hh:mm tt")

    printfn "%s, %s at %s" greeting thing timeOfDay

    let printTruthTable f =
        printfn "       | true  | false |"
        printfn "       +-------+-------+"
        printfn " true  | %5b | %5b |" (f true true) (f true false)
        printfn " false | %5b | %5b |" (f false true) (f false false)
        printfn "       +-------+-------+"
        printfn ""
        ()

    let vowels = ['a'; 'e'; 'i'; 'o'; 'u']
    let emptyList = []

    //Simple list comprehensions
    let numbersNear x = 
        [
            yield x - 1
            yield x
            yield x + 1
        ]

    //More complex list comprehensions
    let x =
        [ let negate x = -x
          for i in 1 .. 10 do
            if i%2 = 0 then
                yield negate i
            else
                yield i ]

    //Using ->
    let multiplesOf x = [ for i in 1 .. 10 do yield x * i]
    let multiplesOf2 x = [ for i in 1 .. 10 -> x * i]

    //List comprehension for prime numbers
    let primesUnder max = 
        [
            for n in 1 .. max do
                let factorsOfN = 
                    [
                        for i in 1 .. n do
                            if n % i = 0 then
                                yield i
                    ]
                    
                //n is prime if it only has 2 factors
                if List.length factorsOfN = 2 then
                    yield n
        ]

    //Using List.partition
    let isMultipleOf5 x = (x%5 = 0)
    let mult5, nonMult5 = 
        List.partition isMultipleOf5 [1 .. 15]

    //Using List.reduce (accumulator has same type as list)
    let insertCommas (acc: string) item = acc + ", " + item
    let conc = List.reduce insertCommas ["Jack"; "Jill"; "Jim"; "Joe"]

    //Using List.fold (accumulator can have different type than list)
    let addAccToListItem acc i = acc + i
    let result = List.fold addAccToListItem 0 [1; 2; 3]

    //Count the number of vowels in a string using List.fold
    let countVowels (str : string) = 
        let charList = List.ofSeq str

        let accFunc (As, Es, Is, Os, Us) letter =
            if   letter = 'a' then (As + 1, Es, Is, Os, Us)
            elif letter = 'e' then (As, Es + 1, Is, Os, Us)
            elif letter = 'i' then (As, Es, Is + 1, Os, Us)
            elif letter = 'o' then (As, Es, Is, Os + 1, Us)
            elif letter = 'u' then (As, Es, Is, Os, Us + 1)
            else (As, Es, Is, Os, Us)

        List.fold accFunc (0, 0, 0, 0, 0) charList

    //Using List.iter (like map, but returns unit => used for side effects)
    let printNumber x = printfn "Printing %d" x
    let result = List.iter printNumber [1 .. 5]

    //Using option (<=> Myabe monad)
    let isInteger str =
        let success, result = Int32.TryParse(str)
        if success then
            Some(result)
        else
            None

    //Using Option.get
    let isNeg x = (x < 0)

    let containsNeg intList = 
        let filteredList = List.filter isNeg intList
        if List.length filteredList <> 0
        then Some(filteredList)
        else None


    //Lambdas - append text to a file
    let appendFile (fileName : string) (text : string) = 
        use file = new StreamWriter (fileName, true)
        file.WriteLine(text)
        file.Close()

    appendFile @"C:\Users\Victor\Documents\Visual Studio 2013\Projects\HelloWorld\Log.txt" "Processing event Z..."
    
    let appendLogFile = appendFile @"C:\Users\Victor\Documents\Visual Studio 2013\Projects\HelloWorld\Log.txt"

    //Functions returning functions
    let generatePowerOfFunc baseValue =
        (fun exponent -> baseValue ** exponent)
  
    //Recursive functions - calculate factorial
    let rec factorial x =
        if x <= 1 then
            1
        else
            x * factorial (x - 1)

    //Functional for loop
    let rec forLoop body times = 
        if times <= 0 then
            ()
        else
            body()
            forLoop body (times - 1)

    //Mutually recursive functions - use "and"
    let rec isOdd x =
        if x = 0 then false
        elif x = 1 then true
        else isEven (x - 1)
    and isEven x =
        if x = 0 then true
        elif x = 1 then false
        else isOdd (x - 1)
    
    //Symbolic operators - factorial
    let rec (!) x = 
        if x <= 1 then 1
        else x * !(x - 1)

    //Symbolic operators - compare string w/ regex
    //open System.Text.RegularExpressions
    let (===) str (regex : string) = 
        Regex.Match(str, regex).Success

    //Pipe forward operator (|>) - get size of a folder
    let sizeOfFolderPiped folder = 

        let getFiles path = 
            Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)

        let totalSize =
            folder
            |> getFiles
            |> Array.map (fun file -> new FileInfo(file))
            |> Array.map (fun info -> info.Length)
            |> Array.sum

        totalSize

    //Forward composition operator (>>) - get size of a folder
    let sizeOfFolderComposed=

        let getFiles folder = 
            Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)

        getFiles
        >> Array.map (fun file -> new FileInfo(file))
        >> Array.map (fun info -> info.Length)
        >> Array.sum

    //Pipe backward operator (<|)
    printfn "The result of sprintf is %s" <| sprintf "(%d, %d)" 1 2

    //Backward composition operator
    let square x = x * x
    let negate x = -x

    (square >> negate) 10 |> ignore //-100 [negation of the square]
    (square << negate) 10 |> ignore //100 [square of the negation]

    //Another example: filtering empty lists
    [ [1]; []; [4;5;6]; [3;4]; []; []; [9] ]
    |> List.filter (not << List.isEmpty) 
    |> ignore

    //Pattern matching - truth table for AND
    let testAnd x y =
        match x, y with
        | true, true -> true
        | true, false -> false
        | false, true -> false
        | false, false -> false

    let testAndWildcard x y =
        match x, y with
        | true, true -> true
        | _, _ -> false

    //Pattern matching - named patterns
    let greet name =
        match name with
        | "Robert" -> printfn "Hello, Bob"
        | "William" -> printfn "Hello, Bill"
        | x -> printfn "Hello, %s" x

    //Pattern matching - matching literals
//    [<Literal>]
//    let Bill = "Bill Gates"

    let greet name =
        match name with
        | Bill -> "Hello, Bill!"
        | x -> sprintf "Hello, %s!" x

    //Grouping patterns (using | and &)
    let vowelTest c =
        match c with
        | 'a' | 'e' | 'i' | 'o' | 'u'
            -> true
        | _ -> false

    let describeNumbers x y =
        match x, y with
        | 1, _
        | _, 1
            -> "One of the numbers is 1."
        | (2, _) & (_, 2)
            -> "Both numbers are 2."
        | _ -> "Other."

    //Patten matching - matching the structure of data
    // 1. Tuples
    let testXor x y =
        match x, y with
        | tuple when fst tuple <> snd tuple
            -> true
        | _ -> false

    // 2. Lists
    let rec listLength l = 
        match l with
        | [] -> 0
        | [_] -> 1
        | [_; _] -> 2
        | x :: xs -> 1 + listLength xs

    //Note: Can also be defined using "function"
    let rec listLength' =
        function
        | [] -> 0
        | [_] -> 1
        | x :: xs -> 1 + listLength' xs

    // 3. Options
    let describeOption x = 
        match x with
        | Some(42) -> "The answer is 42."
        | Some(n) -> sprintf "The answer is %d." n
        | None -> "There is no answer."


    //Discriminated Unions (like Haskell algebraic types)
    let deckOfCards = 
        [
            for suit in [Spade; Club; Heart; Diamond] do
                yield Ace(suit)
                yield King(suit)
                yield Queen(suit)
                yield Jack(suit)
                for value in 2 .. 10 do
                    yield ValueCard(value, suit)
        ]




    // Program exit code
    0
