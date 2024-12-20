using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IHttpClientFactory httpClientFactory, ILogger<ProjectsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync("api/projects");

            if (response.IsSuccessStatusCode)
            {
                var projects = await response.Content.ReadFromJsonAsync<List<ProjectDTO>>();
                return View(projects ?? new List<ProjectDTO>());
            }
            else
            {
                _logger.LogWarning("Ошибка при получении проектов: {StatusCode}", response.StatusCode);
                return View(new List<ProjectDTO>());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка проектов");
            return View(new List<ProjectDTO>());
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("API");
            _logger.LogInformation("Отправка запроса на создание проекта: {@Model}", model);
            
            var response = await client.PostAsJsonAsync("api/projects", model);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Получен ответ от сервера: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(responseContent))
            {
                ModelState.AddModelError(string.Empty, responseContent);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при создании проекта");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании проекта");
            ModelState.AddModelError(string.Empty, "Произошла ошибка при создании проекта");
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/projects/{id}");

            if (response.IsSuccessStatusCode)
            {
                var project = await response.Content.ReadFromJsonAsync<ProjectDTO>();
                if (project != null)
                {
                    var updateModel = new UpdateProjectDTO
                    {
                        Name = project.Name,
                        Description = project.Description
                    };
                    return View(updateModel);
                }
            }
            else
            {
                _logger.LogWarning("Ошибка при получении проекта: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении проекта для редактирования");
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProjectDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.PutAsJsonAsync($"api/projects/{id}", model);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(responseContent))
            {
                ModelState.AddModelError(string.Empty, responseContent);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Произошла ошибка при обновлении проекта");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении проекта");
            ModelState.AddModelError(string.Empty, "Произошла ошибка при обновлении проекта");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"api/projects/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Ошибка при удалении проекта: {StatusCode}, {Content}", 
                    response.StatusCode, responseContent);
                TempData["Error"] = "Произошла ошибка при удалении проекта";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении проекта");
            TempData["Error"] = "Произошла ошибка при удалении проекта";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var projectResponse = await client.GetAsync($"api/projects/{id}");
            var tasksResponse = await client.GetAsync($"api/tasks/project/{id}");

            if (projectResponse.IsSuccessStatusCode && tasksResponse.IsSuccessStatusCode)
            {
                var project = await projectResponse.Content.ReadFromJsonAsync<ProjectDTO>();
                var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskDTO>>();
                
                if (project != null)
                {
                    ViewBag.Tasks = tasks ?? new List<TaskDTO>();
                    return View(project);
                }
            }
            else
            {
                _logger.LogWarning("Ошибка при получении деталей проекта: Project {ProjectStatus}, Tasks {TasksStatus}", 
                    projectResponse.StatusCode, tasksResponse.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении деталей проекта");
        }

        return NotFound();
    }
} 