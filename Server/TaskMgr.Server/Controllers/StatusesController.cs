using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgr.Server.Data;
using TaskMgr.Server.Models;

namespace TaskMgr.Server.Controllers;

/// <summary>
/// Контроллер для работы со статусами задач
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatusesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Конструктор контроллера статусов
    /// </summary>
    public StatusesController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все статусы задач
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Status>>> GetStatuses()
    {
        var statuses = await _context.Statuses.ToListAsync();
        return Ok(statuses);
    }
} 