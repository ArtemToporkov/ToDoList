using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum Priority
{
    [Display(Name = "Простая")]
    Easy,
    [Display(Name = "Средняя")]
    Medium,
    [Display(Name = "Сложная")]
    Hard
}