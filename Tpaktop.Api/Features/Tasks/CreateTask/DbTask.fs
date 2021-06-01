module CreateTask.Entities

open System
open CreateTask.Model
open CreateTask.Types

type CreatedAt = CreatedAt of DateTime

type DbTask =
    { Name: TaskName
      AssignedTo: AssignedTo
      CreatedBy: CreatedBy
      CreatedAt: CreatedAt }

let createTaskFromModel (createTaskModel: CreateTaskModelValidated) : DbTask =
    { Name = createTaskModel.Name
      AssignedTo = createTaskModel.AssignedTo
      CreatedBy = createTaskModel.CreatedBy
      CreatedAt = CreatedAt DateTime.UtcNow }
