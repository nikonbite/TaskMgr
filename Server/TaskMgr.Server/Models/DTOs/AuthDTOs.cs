using System.ComponentModel.DataAnnotations;

namespace TaskMgr.Server.Models.DTOs;

/// <summary>
/// DTO для регистрации пользователя
/// </summary>
public class RegisterDTO
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [Required(ErrorMessage = "Имя обязательно")]
    [MaxLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    [Required(ErrorMessage = "Фамилия обязательна")]
    [MaxLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email пользователя
    /// </summary>
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    [MaxLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO для входа пользователя
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// Email пользователя
    /// </summary>
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO с токеном аутентификации
/// </summary>
public class AuthTokenDTO
{
    /// <summary>
    /// JWT токен
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Время истечения токена
    /// </summary>
    public DateTime Expiration { get; set; }
} 