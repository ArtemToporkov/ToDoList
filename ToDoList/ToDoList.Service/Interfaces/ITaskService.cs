using ToDoList.Domain.Entity;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Interfaces;

public interface ITaskService
{
    public Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model);
}