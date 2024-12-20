namespace TaskMgr.Client.Models;

/// <summary>
/// DTO для отображения информации о статусе задачи
/// </summary>
public class StatusDTO
{
    /// <summary>
    /// Идентификатор статуса
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Название статуса
    /// </summary>
    public string Name { get; set; } = string.Empty;
} 