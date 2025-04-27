using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum StatusCode
{
    OK = 200,
    InternalServerError = 500,
    TaskAlreadyExists = 1,
}