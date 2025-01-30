namespace ToDoList.Domain.Enum;

public enum StatusCode
{
    TaskExistsAlready,
    OK = 200,
    InternalServerError = 500
}