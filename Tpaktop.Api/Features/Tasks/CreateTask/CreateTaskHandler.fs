module CreateTask.Handler

open CreateTask.Model
open Giraffe
open Database
open Tpaktop.Constants

let createTask task =
     create dbConnStr task |> Async.RunSynchronously  

let createTaskHandler (unvalidatedTask: CreateTaskModelUnvalidated): HttpHandler =
    match createValidatedTask unvalidatedTask with
        | Ok validatedTask -> createTask validatedTask |> Successful.OK 
        | Error e -> RequestErrors.BAD_REQUEST $"{e}"       