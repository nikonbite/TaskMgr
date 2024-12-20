using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskMgr.Server.Data;
using TaskMgr.Server.Models;
using TaskMgr.Server.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace TaskMgr.Server.Controllers;

/// <summary>
/// Контроллер для работы с проектами
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProjectsController> _logger;

    /// <summary>
    /// Конструктор контроллера проектов
    /// </summary>
    public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<ProjectsController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Получить все проекты текущего пользователя
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();
        
        var projects = await _context.Projects
            .Include(p => p.Owner)
            .Where(p => p.OwnerID == user.Id)
            .Select(p => new ProjectDTO
            {
                ID = p.ID,
                OwnerID = p.OwnerID,
                OwnerName = p.Owner.FullName,
                Name = p.Name,
                Description = p.Description ?? string.Empty,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return Ok(projects);
    }

    /// <summary>
    /// Получить проект по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDTO>> GetProject(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();
        
        var project = await _context.Projects
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.ID == id);

        if (project == null)
        {
            return NotFound();
        }

        // Проверяем, что пользователь имеет доступ к проекту
        if (project.OwnerID != user.Id)
        {
            return Forbid();
        }

        var projectDto = new ProjectDTO
        {
            ID = project.ID,
            OwnerID = project.OwnerID,
            OwnerName = project.Owner.FullName,
            Name = project.Name,
            Description = project.Description ?? string.Empty,
            CreatedAt = project.CreatedAt
        };

        return Ok(projectDto);
    }

    /// <summary>
    /// Создать новый проект
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProjectDTO>> CreateProject(CreateProjectDTO createProjectDto)
    {
        try
        {
            _logger.LogInformation("Получен запрос на создание проекта: {@CreateProjectDto}", createProjectDto);
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Пользователь не найден");
                return Unauthorized("Пользователь не найден");
            }

            var project = new Project
            {
                OwnerID = user.Id,
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Загружаем владельца для получения его имени
            await _context.Entry(project)
                .Reference(p => p.Owner)
                .LoadAsync();

            var projectDto = new ProjectDTO
            {
                ID = project.ID,
                OwnerID = project.OwnerID,
                OwnerName = project.Owner.FullName,
                Name = project.Name,
                Description = project.Description ?? string.Empty,
                CreatedAt = project.CreatedAt
            };

            _logger.LogInformation("Проект успешно создан: {@ProjectDto}", projectDto);
            return CreatedAtAction(nameof(GetProject), new { id = project.ID }, projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании проекта");
            return StatusCode(500, "Внутренняя ошибка сервера при создании проекта");
        }
    }

    /// <summary>
    /// Обновить проект
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, UpdateProjectDTO updateProjectDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();
        
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        // Проверяем, что пользователь имеет доступ к проекту
        if (project.OwnerID != user.Id)
        {
            return Forbid();
        }

        project.Name = updateProjectDto.Name;
        project.Description = updateProjectDto.Description;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить проект
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();
        
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        // Проверяем, что пользователь имеет доступ к проекту
        if (project.OwnerID != user.Id)
        {
            return Forbid();
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
} 