using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMgr.Server.Models;

/// <summary>
/// Модель задачи
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Уникальный идентификатор задачи
    /// </summary>
    [Key]
    public int ID { get; set; }
    
    /// <summary>
    /// Заголовок задачи
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Описание задачи
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Идентификатор проекта
    /// </summary>
    [Required]
    public int ProjectID { get; set; }
    
    /// <summary>
    /// Проект, к которому относится задача
    /// </summary>
    [ForeignKey("ProjectID")]
    public Project Project { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор исполнителя
    /// </summary>
    public string? AssigneeID { get; set; }
    
    /// <summary>
    /// Исполнитель задачи
    /// </summary>
    [ForeignKey("AssigneeID")]
    public ApplicationUser? Assignee { get; set; }
    
    /// <summary>
    /// Идентификатор статуса
    /// </summary>
    [Required]
    public int StatusID { get; set; }
    
    /// <summary>
    /// Статус задачи
    /// </summary>
    [ForeignKey("StatusID")]
    public Status Status { get; set; } = null!;
    
    /// <summary>
    /// Крайний срок выполнения задачи
    /// </summary>
    public DateTime? Deadline { get; set; }
    
    /// <summary>
    /// Дата создания задачи
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Дата последнего обновления задачи
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 