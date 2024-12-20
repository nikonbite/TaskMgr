using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMgr.Server.Models;

/// <summary>
/// Модель проекта
/// </summary>
public class Project
{
    /// <summary>
    /// Уникальный идентификатор проекта
    /// </summary>
    [Key]
    public int ID { get; set; }
    
    /// <summary>
    /// Название проекта
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Описание проекта
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Дата создания проекта
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Идентификатор владельца проекта
    /// </summary>
    [Required]
    public string OwnerID { get; set; } = string.Empty;
    
    /// <summary>
    /// Владелец проекта
    /// </summary>
    [ForeignKey("OwnerID")]
    public ApplicationUser Owner { get; set; } = null!;
    
    /// <summary>
    /// Задачи в проекте
    /// </summary>
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    
    /// <summary>
    /// Участники проекта
    /// </summary>
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
} 