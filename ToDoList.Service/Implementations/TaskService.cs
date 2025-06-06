using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extensions;
using ToDoList.Domain.Filters.Task;

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

    public async Task<DataTableResult> GetTasks(TaskFilter filter)
    {
        try 
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => !x.IsDone)
                .WhereIf(!string.IsNullOrEmpty(filter.Name), x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
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
            var count = _taskRepository.GetAll().Count(x => !x.IsDone);

            return new DataTableResult
            {
                Data = tasks,
                Total = count
            };
        }
        catch (Exception ex) 
        {
           _logger.LogError(ex, $"[TaskService.GetTasks]: {ex.Message}");
            return new DataTableResult();
        }
    }

    public async Task<IBaseResponse<bool>> EndTask(long id) 
    {
        try 
        {
            var task = await _taskRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (task is null) 
            {
                return new BaseResponse<bool>() 
                {
                    Description = "Задача не найдена",
                    StatusCode = StatusCode.TaskNotFound
                };
            }
            task.IsDone = true;
            await _taskRepository.Update(task);
            return new BaseResponse<bool>() 
            {
                Description = "Задача успешно завершена",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, $"[TaskService.EndTask]: {ex.Message}");
            return new BaseResponse<bool>() 
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskCompletedViewModel>>> GetCompletedTasks()
    {
        try 
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.IsDone)
                .Where(x => x.Created.Date == DateTime.UtcNow.Date)
                .Select(x => new TaskCompletedViewModel
                {
                    Name = x.Name,
                    Description = x.Description
                })
                .ToListAsync();

            return new BaseResponse<IEnumerable<TaskCompletedViewModel>>
            {
                Data = tasks,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, $"[TaskService.GetCompletedTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskCompletedViewModel>>
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks()
    {
        try 
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.UtcNow.Date)
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

            return new BaseResponse<IEnumerable<TaskViewModel>>
            {
                Data = tasks,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, $"[TaskService.CalculateCompletedTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>
            {
                Description = ex.Message,
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}