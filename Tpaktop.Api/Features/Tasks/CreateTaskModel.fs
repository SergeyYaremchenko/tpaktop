module CreateTaskModel

type TaskName = TaskName of string
type AssignedTo = AssignedTo of string option
type CreatedBy = CreatedBy of string

type CreateTaskValidationFailure =
    | NameIsInvalid of string
    | CreatedByIsInvalid of string
    | EmptyModel of string

let (|InvalidTaskName|_|) (name:string) =
    if name = null || name.Length < 2 then Some "Name should be at least 3 characters long" else None 

let (|InvalidCreatedBy|_|) (createdBy: string) =
    match createdBy with
        | TryParser.Guid _ -> None
        | _ -> Some "CreatedBy should be a valid UUIDv4"

let tryGetName input =
    match input with
        | InvalidTaskName msg -> Error [ NameIsInvalid msg ]
        | valid -> Ok (TaskName valid)
        
let tryGetCreatedBy input =
    match input with
        | InvalidCreatedBy msg -> Error [ CreatedByIsInvalid msg ]
        | valid -> Ok (CreatedBy valid)        

[<CLIMutable>]
type CreateTaskModelUnvalidated =
    { Name: string
      AssignedTo: string option
      CreatedBy: string }
    
type CreateTaskModelValidated =
    { Name: TaskName
      AssignedTo: AssignedTo
      CreatedBy: CreatedBy }
    
let createTask (input: CreateTaskModelUnvalidated) =
    let inputOption = if(box input = null) then None else Some input
    
    match inputOption with
        | None -> Error [ EmptyModel "Model is empty" ]
        | Some i -> 
            let validatedName = i.Name |> tryGetName

            let validatedCreatedBy = i.CreatedBy |> tryGetCreatedBy

            let result =
                match validatedName, validatedCreatedBy with
                    | Ok n, Ok c -> Ok { Name = n; AssignedTo = AssignedTo input.AssignedTo; CreatedBy = c }
                    | Error ex, Ok _ -> Error ex
                    | Ok _, Error ex -> Error ex
                    | Error ex1, Error ex2 -> Error (List.concat [ex1; ex2])
                    
            result