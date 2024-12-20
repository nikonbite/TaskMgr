using System.ComponentModel.DataAnnotations;

namespace TaskMgr.Server.Models;

/// <summary>
/// Модель пользователя системы
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public long ID { get; set; }
    
    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Email пользователя (используется для входа)
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Хэш пароля пользователя
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Проекты, созданные пользователем
    /// </summary>
    public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    
    /// <summary>
    /// Задачи, назначенные пользователю
    /// </summary>
    public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
} 