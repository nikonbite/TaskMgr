using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMgr.Server.Models;

public class ProjectMember
{
    [Key]
    public int ID { get; set; }

    [Required]
    public int ProjectID { get; set; }

    [ForeignKey("ProjectID")]
    public Project Project { get; set; } = null!;

    [Required]
    public string UserID { get; set; } = string.Empty;

    [ForeignKey("UserID")]
    public ApplicationUser User { get; set; } = null!;

    [Required]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public ProjectRole Role { get; set; } = ProjectRole.Member;
} 