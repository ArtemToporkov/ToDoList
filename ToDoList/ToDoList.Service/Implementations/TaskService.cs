﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.DAL.Repositories;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

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
            _logger.LogInformation($"Запрос на создание задачи - {model.Name}");
            var task = await _taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.Today)
                .FirstOrDefaultAsync(x => x.Name == model.Name);
            if (task is not null)
            {
                return new BaseResponse<TaskEntity>
                {
                    Description = "Задача с таким названием уже есть.",
                    StatusCode = StatusCode.TaskExistsAlready
                };
            }

            task = new TaskEntity
            {
                Name = model.Name,
                Description = model.Description,
                Priority = model.Priority,
                Created = DateTime.Now,
                IsDone = false
            };

            await _taskRepository.Create(task);
            
            _logger.LogInformation($"Задача создалась - {task.Name} {task.Created}");
            return new BaseResponse<TaskEntity>
            {
                Description = "Задача создалась",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[TaskService.Create]: {e.Message}");
            return new BaseResponse<TaskEntity>
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}