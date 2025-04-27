using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum Priority
{
    [Display(Name = "Неважная")]
    Easy = 1,
    [Display(Name = "Обычная")]
    Medium = 2,
    [Display(Name = "Критическая")]
    Hard = 3
}