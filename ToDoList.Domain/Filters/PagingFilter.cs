namespace ToDoList.Domain.Filters;

public class PagingFilter
{
    public int PageSize { get; set; }

    public int Skip { get; set; }
}