module CreateTaskHandler

open CreateTaskModel
open Giraffe

let createTaskHandler (unvalidatedTask: CreateTaskModelUnvalidated): HttpHandler =
    match createTask unvalidatedTask with
        | Ok verifiedTask -> Successful.OK verifiedTask
        | Error e -> RequestErrors.BAD_REQUEST $"{e}"
        
