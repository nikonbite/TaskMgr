using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgr.Server.Models;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Server.Controllers;

/// <summary>
/// Контроллер для работы с пользователями
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Конструктор контроллера пользователей
    /// </summary>
    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Получить список всех пользователей (для назначения задач)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        var users = await _userManager.Users
            .Select(u => new UserDTO
            {
                ID = u.Id,
                UserName = u.UserName ?? string.Empty,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Получить информацию о пользователе по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var userDto = new UserDTO
        {
            ID = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty
        };

        return Ok(userDto);
    }
} 