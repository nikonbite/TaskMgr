using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TaskMgr.Server.Models.DTOs;
using TaskMgr.Client.Models;

namespace TaskMgr.Client.Controllers;

[Authorize]
public class TasksController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TasksController> _logger;

    public TasksController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TasksController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync("api/tasks/my");

        if (response.IsSuccessStatusCode)
        {
            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDTO>>();
            return View(tasks ?? new List<TaskDTO>());
        }

        return View(new List<TaskDTO>());
    }

    public async Task<IActionResult> Create(long projectId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var usersResponse = await client.GetAsync("api/users");

        if (usersResponse.IsSuccessStatusCode)
        {
            try 
            {
                var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDTO>>();
                if (users != null)
                {
                    ViewBag.Users = users;
                    ViewBag.ProjectId = projectId;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при десериализации списка пользователей");
                var content = await usersResponse.Content.ReadAsStringAsync();
                _logger.LogError("Содержимое ответа: {Content}", content);
            }
        }

        return RedirectToAction("Details", "Projects", new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTaskDTO model)
    {
        if (!ModelState.IsValid)
        {
            var client = _httpClientFactory.CreateClient("API");
            var usersResponse = await client.GetAsync("api/users");
            if (usersResponse.IsSuccessStatusCode)
            {
                var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDTO>>();
                if (users != null)
                {
                    ViewBag.Users = users;
                }
            }
            return View(model);
        }

        var httpClient = _httpClientFactory.CreateClient("API");
        var response = await httpClient.PostAsJsonAsync("api/tasks", model);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Details", "Projects", new { id = model.ProjectID });
        }

        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (error != null && error.ContainsKey("message"))
        {
            ModelState.AddModelError(string.Empty, error["message"]);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Произошла ошибка при создании задачи");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var client = _httpClientFactory.CreateClient("API");
        var taskResponse = await client.GetAsync($"api/tasks/{id}");
        var usersResponse = await client.GetAsync("api/users");
        var statusesResponse = await client.GetAsync("api/statuses");

        if (taskResponse.IsSuccessStatusCode && usersResponse.IsSuccessStatusCode && statusesResponse.IsSuccessStatusCode)
        {
            var task = await taskResponse.Content.ReadFromJsonAsync<TaskDTO>();
            var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDTO>>();
            var statuses = await statusesResponse.Content.ReadFromJsonAsync<List<StatusDTO>>();

            if (task != null && users != null && statuses != null)
            {
                ViewBag.Users = users;
                ViewBag.Statuses = statuses;
                ViewBag.TaskId = id;

                var updateModel = new UpdateTaskDTO
                {
                    Title = task.Title,
                    Description = task.Description,
                    AssigneeID = task.AssigneeID,
                    StatusID = task.StatusID,
                    Deadline = task.Deadline
                };

                return View(updateModel);
            }
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, UpdateTaskDTO model)
    {
        if (!ModelState.IsValid)
        {
            var client = _httpClientFactory.CreateClient("API");
            var usersResponse = await client.GetAsync("api/users");
            var statusesResponse = await client.GetAsync("api/statuses");

            if (usersResponse.IsSuccessStatusCode && statusesResponse.IsSuccessStatusCode)
            {
                var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDTO>>();
                var statuses = await statusesResponse.Content.ReadFromJsonAsync<List<StatusDTO>>();
                if (users != null && statuses != null)
                {
                    ViewBag.Users = users;
                    ViewBag.Statuses = statuses;
                }
            }

            return View(model);
        }

        var httpClient = _httpClientFactory.CreateClient("API");
        var response = await httpClient.PutAsJsonAsync($"api/tasks/{id}", model);

        if (response.IsSuccessStatusCode)
        {
            var taskResponse = await httpClient.GetAsync($"api/tasks/{id}");
            if (taskResponse.IsSuccessStatusCode)
            {
                var task = await taskResponse.Content.ReadFromJsonAsync<TaskDTO>();
                if (task != null)
                {
                    return RedirectToAction("Details", "Projects", new { id = task.ProjectID });
                }
            }
            return RedirectToAction(nameof(Index));
        }

        var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (error != null && error.ContainsKey("message"))
        {
            ModelState.AddModelError(string.Empty, error["message"]);
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Произошла ошибка при обновлении задачи");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var client = _httpClientFactory.CreateClient("API");
        
        var taskResponse = await client.GetAsync($"api/tasks/{id}");
        long? projectId = null;
        
        if (taskResponse.IsSuccessStatusCode)
        {
            var task = await taskResponse.Content.ReadFromJsonAsync<TaskDTO>();
            if (task != null)
            {
                projectId = task.ProjectID;
            }
        }

        var response = await client.DeleteAsync($"api/tasks/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Произошла ошибка при удалении задачи";
        }

        if (projectId.HasValue)
        {
            return RedirectToAction("Details", "Projects", new { id = projectId.Value });
        }

        return RedirectToAction(nameof(Index));
    }
} 