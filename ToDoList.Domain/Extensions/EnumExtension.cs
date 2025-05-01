using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ToDoList.Domain.Extensions;

public static class EnumExtension
{
    public static string GetDisplayName(this System.Enum value)
    {
        return value.GetType()
            .GetMember(value.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.GetName() ?? "Неопределённый";
    }
}