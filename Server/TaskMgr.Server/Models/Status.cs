using System.ComponentModel.DataAnnotations;

namespace TaskMgr.Server.Models;

/// <summary>
/// Модель статуса задачи
/// </summary>
public class Status
{
    /// <summary>
    /// Уникальный идентификатор статуса
    /// </summary>
    public int ID { get; set; }
    
    /// <summary>
    /// Название статуса
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Задачи с данным статусом
    /// </summary>
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
} 