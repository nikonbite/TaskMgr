using System.ComponentModel.DataAnnotations;

namespace TaskMgr.Server.Models.DTOs;

/// <summary>
/// DTO для создания задачи
/// </summary>
public class CreateTaskDTO
{
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    [Required(ErrorMessage = "Идентификатор проекта обязателен")]
    public int ProjectID { get; set; }

    /// <summary>
    /// Идентификатор исполнителя (опционально)
    /// </summary>
    public string? AssigneeID { get; set; }

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    [Required(ErrorMessage = "Заголовок задачи обязателен")]
    [MaxLength(200, ErrorMessage = "Заголовок задачи не должен превышать 200 символов")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание задачи
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Крайний срок выполнения задачи
    /// </summary>
    public DateTime? Deadline { get; set; }
}

/// <summary>
/// DTO для обновления задачи
/// </summary>
public class UpdateTaskDTO
{
    /// <summary>
    /// Идентификатор исполнителя (опционально)
    /// </summary>
    public string? AssigneeID { get; set; }

    /// <summary>
    /// Идентификатор статуса
    /// </summary>
    public int StatusID { get; set; }

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    [Required(ErrorMessage = "Заголовок задачи обязателен")]
    [MaxLength(200, ErrorMessage = "Заголовок задачи не должен превышать 200 символов")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание задачи
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Крайний срок выполнения задачи
    /// </summary>
    public DateTime? Deadline { get; set; }
}

/// <summary>
/// DTO для отображения задачи
/// </summary>
public class TaskDTO
{
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// Название проекта
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор исполнителя
    /// </summary>
    public string? AssigneeID { get; set; }

    /// <summary>
    /// Имя исполнителя
    /// </summary>
    public string? AssigneeName { get; set; }

    /// <summary>
    /// Идентификатор статуса
    /// </summary>
    public int StatusID { get; set; }

    /// <summary>
    /// Название статуса
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// Заголовок задачи
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание задачи
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания задачи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Крайний срок выполнения задачи
    /// </summary>
    public DateTime? Deadline { get; set; }
} 