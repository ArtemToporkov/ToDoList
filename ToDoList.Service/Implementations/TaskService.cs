using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extensions;

namespace ToDoList.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<TaskEntity> _taskRepository;
    private ILogger<TaskService> _logger;
    
    public TaskService(IBaseRepository<TaskEntity> taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }
    
    public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
    {
        try
        {
            model.Validate();

            _logger.LogInformation($"Запрос на создание задачи: {model.Name}");
            var task = await _taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.UtcNow.Date)
                .FirstOrDefaultAsync(x => x.Name == model.Name);
            if (task is not null) 
            {
                return new BaseResponse<TaskEntity>() 
                {
                    Description = "Задача с таким именем уже существует",
                    StatusCode = StatusCode.TaskAlreadyExists
                };
            }

            task = new TaskEntity() 
            {
                Name = model.Name,
                Description = model.Description,
                Priority = model.Priority,
                Created = DateTime.UtcNow,
                IsDone = false
            };
            await _taskRepository.Create(task);
            return new BaseResponse<TaskEntity>() 
            {
                Description = "Задача успешно создана",
                StatusCode = StatusCode.OK,
                Data = task
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.Create]: {ex.Message}");
            return new BaseResponse<TaskEntity>() 
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks()
    {
        try 
        {
            var tasks = await _taskRepository.GetAll()
                .Select(x => new TaskViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDone = x.IsDone == true ? "Готова" : "Не готова",
                    Priority = x.Priority.GetDisplayName(),
                    Description = x.Description,
                    Created = x.Created.ToLongDateString()
                })
                .ToListAsync();

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex) 
        {
           _logger.LogError(ex, $"[TaskService.GetTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>() 
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            }; 
        }
    }
}