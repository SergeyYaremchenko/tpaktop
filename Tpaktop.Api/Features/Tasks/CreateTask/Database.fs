module Database

open System
open CreateTask.Model
open CreateTask.Entities
open CreateTask.Types
open Tpaktop.Operators
open System.Data.SQLite
open Dapper

let get (connStr: string) (id: int) : Async<DbTask option> =
    let sql = "SELECT * FROM tasks WHERE Id = @id"
    // Parameterize the id into a Dictionary<string, object>
    let data = [ "id" => id ]

    // Execute an async block which connects to the database and maps the result
    async {
        // Connect to the SQL database
        use conn = new SQLiteConnection(connStr)
        conn.OpenAsync() |> Async.AwaitTask |> ignore

        let! dbResult =
            conn.QuerySingleOrDefaultAsync<DbTask>(sql, data)
            |> Async.AwaitTask

        conn.CloseAsync() |> Async.AwaitTask |> ignore

        let result =
            if box dbResult = null then
                None
            else
                Some dbResult

        return result
    }

let create (connStr: string) (task: CreateTaskModelValidated) : Async<bigint> =
    let sql =
        "insert into tasks(AssignedTo, CreatedBy, CreatedAt) values(@assignedTo, @createdBy, @createdAt)"

    let dbTask = createTaskFromModel task

    let (AssignedTo assignedTo') = dbTask.AssignedTo
    let (CreatedBy createdBy') = dbTask.CreatedBy
    let (CreatedAt createdAt') = dbTask.CreatedAt
    let (TaskName name') = dbTask.Name
    
    let data =
        dict [ "assignedTo" => assignedTo'; "createdBy" => createdBy'; "createdAt" => createdAt'; "name" => name' ]

    async {
        // Connect to the SQL database
        use conn = new SQLiteConnection(connStr)
        conn.OpenAsync() |> Async.AwaitTask |> ignore

        let! result =
            conn.QuerySingleAsync<bigint>(sql, data)
            |> Async.AwaitTask

        conn.CloseAsync() |> Async.AwaitTask |> ignore

        return result
    }
