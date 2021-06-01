module Tpaktop.Migrator.Migrations

open FluentMigrator

[<Migration(20213105080000L)>]
type AddTasks() =
    inherit Migration()
    
    override m.Up() =
        m.Create.Table("tasks")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("AssignedTo").AsString()
            .WithColumn("CreatedBy").AsString()
            .WithColumn("CreatedAt").AsDateTime()
            |> ignore
    
    override m.Down() =
        m.Delete.Table("tasks")
        |> ignore
        
