using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskMgr.Server.Models;

namespace TaskMgr.Server.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Конструктор контекста
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Проекты
    /// </summary>
    public DbSet<Project> Projects => Set<Project>();
    
    /// <summary>
    /// Задачи
    /// </summary>
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    
    /// <summary>
    /// Статусы задач
    /// </summary>
    public DbSet<Status> Statuses => Set<Status>();
    
    /// <summary>
    /// Члены проекта
    /// </summary>
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    /// <summary>
    /// Настройка моделей при создании
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Добавляем базовые статусы
        modelBuilder.Entity<Status>().HasData(
            new Status { ID = 1, Name = "К выполнению" },
            new Status { ID = 2, Name = "В работе" },
            new Status { ID = 3, Name = "На проверке" },
            new Status { ID = 4, Name = "Готово" }
        );

        // Настройка каскадного удаления для проектов
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithOne(m => m.Project)
            .HasForeignKey(m => m.ProjectID)
            .OnDelete(DeleteBehavior.NoAction);

        // Настройка уникальности участников проекта
        modelBuilder.Entity<ProjectMember>()
            .HasIndex(m => new { m.ProjectID, m.UserID })
            .IsUnique();
    }
} 