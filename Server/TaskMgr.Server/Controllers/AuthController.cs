using Microsoft.AspNetCore.Mvc;
using TaskMgr.Server.Models.DTOs;
using TaskMgr.Server.Services;

namespace TaskMgr.Server.Controllers;

/// <summary>
/// Контроллер аутентификации
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Конструктор контроллера аутентификации
    /// </summary>
    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Регистрация нового пользователя
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        try
        {
            _logger.LogInformation("Попытка регистрации пользователя: {Email}", model.Email);
            
            var result = await _authService.RegisterAsync(model);
            if (!result.success)
            {
                _logger.LogWarning("Ошибка при регистрации пользователя {Email}: {Message}", 
                    model.Email, result.message);
                return BadRequest(result.message);
            }

            _logger.LogInformation("Пользователь {Email} успешно зарегистрирован", model.Email);
            return Ok(new { token = result.token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Email}", model.Email);
            return StatusCode(500, "Внутренняя ошибка сервера при регистрации");
        }
    }

    /// <summary>
    /// Вход пользователя
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        try
        {
            _logger.LogInformation("Попытка входа пользователя: {Email}", model.Email);
            
            var result = await _authService.LoginAsync(model);
            if (!result.success)
            {
                _logger.LogWarning("Ошибка при входе пользователя {Email}: {Message}", 
                    model.Email, result.message);
                return BadRequest(result.message);
            }

            _logger.LogInformation("Пользователь {Email} успешно вошел в систему", model.Email);
            return Ok(new { token = result.token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при входе пользователя {Email}", model.Email);
            return StatusCode(500, "Внутренняя ошибка сервера при входе");
        }
    }
} 