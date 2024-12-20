using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskMgr.Server.Data;
using TaskMgr.Server.Models;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Server.Controllers;

/// <summary>
/// Контроллер для работы с задачами
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Конструктор контроллера задач
    /// </summary>
    public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Получить все задачи в проекте
    /// </summary>
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<IEnumerable<TaskDTO>>> GetProjectTasks(int projectId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        // Проверяем доступ к проекту
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Проект не найден");
        }
        if (project.OwnerID != user.Id)
        {
            return Forbid();
        }

        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Status)
            .Where(t => t.ProjectID == projectId)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                ProjectID = t.ProjectID,
                ProjectName = t.Project.Name,
                AssigneeID = t.AssigneeID,
                AssigneeName = t.Assignee != null ? t.Assignee.FullName : null,
                StatusID = t.StatusID,
                StatusName = t.Status.Name,
                Title = t.Title,
                Description = t.Description ?? string.Empty,
                CreatedAt = t.CreatedAt,
                Deadline = t.Deadline
            })
            .ToListAsync();

        return Ok(tasks);
    }

    /// <summary>
    /// Получить все задачи, назначенные текущему пользователю
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<TaskDTO>>> GetMyTasks()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Status)
            .Where(t => t.AssigneeID == user.Id)
            .Select(t => new TaskDTO
            {
                ID = t.ID,
                ProjectID = t.ProjectID,
                ProjectName = t.Project.Name,
                AssigneeID = t.AssigneeID,
                AssigneeName = t.Assignee != null ? t.Assignee.FullName : null,
                StatusID = t.StatusID,
                StatusName = t.Status.Name,
                Title = t.Title,
                Description = t.Description ?? string.Empty,
                CreatedAt = t.CreatedAt,
                Deadline = t.Deadline
            })
            .ToListAsync();

        return Ok(tasks);
    }

    /// <summary>
    /// Получить задачу по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDTO>> GetTask(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .Include(t => t.Status)
            .FirstOrDefaultAsync(t => t.ID == id);

        if (task == null)
        {
            return NotFound();
        }

        // Проверяем доступ к задаче (владелец проекта или исполнитель)
        var project = await _context.Projects.FindAsync(task.ProjectID);
        if (project?.OwnerID != user.Id && task.AssigneeID != user.Id)
        {
            return Forbid();
        }

        var taskDto = new TaskDTO
        {
            ID = task.ID,
            ProjectID = task.ProjectID,
            ProjectName = task.Project.Name,
            AssigneeID = task.AssigneeID,
            AssigneeName = task.Assignee?.FullName,
            StatusID = task.StatusID,
            StatusName = task.Status.Name,
            Title = task.Title,
            Description = task.Description ?? string.Empty,
            CreatedAt = task.CreatedAt,
            Deadline = task.Deadline
        };

        return Ok(taskDto);
    }

    /// <summary>
    /// Создать новую задачу
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TaskDTO>> CreateTask(CreateTaskDTO createTaskDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        // Проверяем доступ к проекту
        var project = await _context.Projects.FindAsync(createTaskDto.ProjectID);
        if (project == null)
        {
            return NotFound("Проект не найден");
        }
        if (project.OwnerID != user.Id)
        {
            return Forbid();
        }

        var task = new TaskItem
        {
            ProjectID = createTaskDto.ProjectID,
            AssigneeID = createTaskDto.AssigneeID,
            StatusID = 1, // "К выполнению"
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Deadline = createTaskDto.Deadline
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Загружаем связанные данные
        await _context.Entry(task)
            .Reference(t => t.Project)
            .LoadAsync();
        await _context.Entry(task)
            .Reference(t => t.Assignee)
            .LoadAsync();
        await _context.Entry(task)
            .Reference(t => t.Status)
            .LoadAsync();

        var taskDto = new TaskDTO
        {
            ID = task.ID,
            ProjectID = task.ProjectID,
            ProjectName = task.Project.Name,
            AssigneeID = task.AssigneeID,
            AssigneeName = task.Assignee?.FullName,
            StatusID = task.StatusID,
            StatusName = task.Status.Name,
            Title = task.Title,
            Description = task.Description ?? string.Empty,
            CreatedAt = task.CreatedAt,
            Deadline = task.Deadline
        };

        return CreatedAtAction(nameof(GetTask), new { id = task.ID }, taskDto);
    }

    /// <summary>
    /// Обновить задачу
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDTO updateTaskDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ID == id);

        if (task == null)
        {
            return NotFound();
        }

        // Проверяем доступ к задаче
        if (task.Project.OwnerID != user.Id && task.AssigneeID != user.Id)
        {
            return Forbid();
        }

        // Проверяем существование статуса
        if (!await _context.Statuses.AnyAsync(s => s.ID == updateTaskDto.StatusID))
        {
            return BadRequest("Указанный статус не существует");
        }

        task.AssigneeID = updateTaskDto.AssigneeID;
        task.StatusID = updateTaskDto.StatusID;
        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.Deadline = updateTaskDto.Deadline;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить задачу
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.ID == id);

        if (task == null)
        {
            return NotFound();
        }

        // Проверяем доступ к задаче (только владелец проекта может удалять задачи)
        if (task.Project.OwnerID != user.Id)
        {
            return Forbid();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
} 