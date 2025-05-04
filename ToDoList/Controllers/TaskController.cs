using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Helpers;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers;

public class TaskController : Controller
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> CalculateCompletedTasks()
    {
        var response = await _taskService.CalculateCompletedTasks();
        if (response.StatusCode == Domain.Enum.StatusCode.OK) 
        {
            var csvService = new CsvBaseService<IEnumerable<TaskViewModel>>();
            var uploadFile = csvService.UploadFiles(response.Data);
            return File(uploadFile, "text/csv", $"Стастистика за {DateTime.UtcNow.ToLongDateString()}.csv");
        }

        return BadRequest(new { description = response.Description });
    }

    public async Task<IActionResult> GetCompletedTasks()
    {
        var response = await _taskService.GetCompletedTasks();
        return Json(new { data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        var response = await _taskService.Create(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK) 
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var response = await _taskService.GetTasks(filter);
        return Json(new { data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> EndTask(long id) 
    {
        var response = await _taskService.EndTask(id);
        if (response.StatusCode == Domain.Enum.StatusCode.OK) 
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
}