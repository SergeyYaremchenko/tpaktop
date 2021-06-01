module CreateTask.Types

type TaskName = TaskName of string
type AssignedTo = AssignedTo of string option
type CreatedBy = CreatedBy of string
type TaskId = TaskId of bigint

