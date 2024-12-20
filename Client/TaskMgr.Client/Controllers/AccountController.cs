using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Controllers;

/// <summary>
/// Контроллер для аутентификации
/// </summary>
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Конструктор контроллера аутентификации
    /// </summary>
    public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Страница входа
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Обработка входа
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDTO model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.PostAsJsonAsync("api/auth/login", model);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Получен ответ от сервера: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, responseContent);
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthTokenDTO>();
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError(string.Empty, "Не удалось получить токен аутентификации");
                return View(model);
            }

            await SignInWithToken(result.Token, returnUrl);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при входе");
            ModelState.AddModelError(string.Empty, $"Ошибка при входе: {ex.Message}");
            return View(model);
        }
    }

    /// <summary>
    /// Страница регистрации
    /// </summary>
    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Обработка регистрации
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDTO model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.PostAsJsonAsync("api/auth/register", model);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation("Получен ответ от сервера: {StatusCode}, Content: {Content}", 
                response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, responseContent);
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthTokenDTO>();
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError(string.Empty, "Не удалось получить токен аутентификации");
                return View(model);
            }

            await SignInWithToken(result.Token, returnUrl);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации");
            ModelState.AddModelError(string.Empty, $"Ошибка при регистрации: {ex.Message}");
            return View(model);
        }
    }

    /// <summary>
    /// Выход из системы
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInWithToken(string token, string? returnUrl)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
        {
            throw new InvalidOperationException("Не удалось декодировать JWT токен");
        }

        var claims = new List<Claim>();
        claims.AddRange(jwtToken.Claims);
        claims.Add(new Claim("access_token", token));

        _logger.LogInformation("Создание ClaimsIdentity с claims: {@Claims}", 
            claims.Select(c => new { Type = c.Type, Value = c.Value }));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
            IsPersistent = true,
            RedirectUri = returnUrl
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("Пользователь успешно аутентифицирован");
    }
} 