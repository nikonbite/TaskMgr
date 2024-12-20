using System.ComponentModel.DataAnnotations;

namespace TaskMgr.Server.Models.DTOs;

/// <summary>
/// DTO для создания проекта
/// </summary>
public class CreateProjectDTO
{
    /// <summary>
    /// Название проекта
    /// </summary>
    [Required(ErrorMessage = "Название проекта обязательно")]
    [MaxLength(100, ErrorMessage = "Название проекта не должно превышать 100 символов")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание проекта
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// DTO для обновления проекта
/// </summary>
public class UpdateProjectDTO : CreateProjectDTO
{
}

/// <summary>
/// DTO для отображения проекта
/// </summary>
public class ProjectDTO
{
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Идентификатор владельца
    /// </summary>
    public string OwnerID { get; set; } = string.Empty;

    /// <summary>
    /// Имя владельца
    /// </summary>
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>
    /// Название проекта
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание проекта
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
} 