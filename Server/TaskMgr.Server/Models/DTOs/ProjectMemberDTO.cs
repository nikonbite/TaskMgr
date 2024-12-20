namespace TaskMgr.Server.Models.DTOs;

public class ProjectMemberDTO
{
    public int ID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public ProjectRole Role { get; set; }
}

public class AddProjectMemberDTO
{
    public string UserID { get; set; } = string.Empty;
    public ProjectRole Role { get; set; } = ProjectRole.Member;
}

public class UpdateProjectMemberDTO
{
    public ProjectRole Role { get; set; }
} 