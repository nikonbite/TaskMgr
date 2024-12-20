using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgr.Server.Data;
using TaskMgr.Server.Models;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/members")]
public class ProjectMembersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectMembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectMemberDTO>>> GetMembers(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.ID == projectId);

        if (project == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        // Проверяем, является ли пользователь участником проекта
        var isMember = project.Members.Any(m => m.UserID == currentUser.Id) || project.OwnerID == currentUser.Id;
        if (!isMember)
            return Forbid();

        var members = project.Members.Select(m => new ProjectMemberDTO
        {
            ID = m.ID,
            UserID = m.UserID,
            UserName = m.User.UserName ?? string.Empty,
            FullName = m.User.FullName,
            Email = m.User.Email ?? string.Empty,
            JoinedAt = m.JoinedAt,
            Role = m.Role
        });

        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectMemberDTO>> AddMember(int projectId, AddProjectMemberDTO dto)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.ID == projectId);

        if (project == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        // Проверяем права на добавление участников
        var currentMember = project.Members.FirstOrDefault(m => m.UserID == currentUser.Id);
        if (project.OwnerID != currentUser.Id && (currentMember == null || currentMember.Role != ProjectRole.Admin))
            return Forbid();

        // Проверяем, существует ли пользователь
        var userToAdd = await _userManager.FindByIdAsync(dto.UserID);
        if (userToAdd == null)
            return BadRequest("Пользователь не найден");

        // Проверяем, не является ли пользователь уже участником
        if (project.Members.Any(m => m.UserID == dto.UserID))
            return BadRequest("Пользователь уже является участником проекта");

        var member = new ProjectMember
        {
            ProjectID = projectId,
            UserID = dto.UserID,
            Role = dto.Role,
            JoinedAt = DateTime.UtcNow
        };

        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();

        var memberDto = new ProjectMemberDTO
        {
            ID = member.ID,
            UserID = userToAdd.Id,
            UserName = userToAdd.UserName ?? string.Empty,
            FullName = userToAdd.FullName,
            Email = userToAdd.Email ?? string.Empty,
            JoinedAt = member.JoinedAt,
            Role = member.Role
        };

        return CreatedAtAction(nameof(GetMembers), new { projectId }, memberDto);
    }

    [HttpPut("{memberId}")]
    public async Task<IActionResult> UpdateMember(int projectId, int memberId, UpdateProjectMemberDTO dto)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.ID == projectId);

        if (project == null)
            return NotFound();

        var member = project.Members.FirstOrDefault(m => m.ID == memberId);
        if (member == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        // Проверяем права на изменение ролей
        var currentMember = project.Members.FirstOrDefault(m => m.UserID == currentUser.Id);
        if (project.OwnerID != currentUser.Id && (currentMember == null || currentMember.Role != ProjectRole.Admin))
            return Forbid();

        member.Role = dto.Role;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{memberId}")]
    public async Task<IActionResult> RemoveMember(int projectId, int memberId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.ID == projectId);

        if (project == null)
            return NotFound();

        var member = project.Members.FirstOrDefault(m => m.ID == memberId);
        if (member == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        // Проверяем права на удаление участников
        var currentMember = project.Members.FirstOrDefault(m => m.UserID == currentUser.Id);
        if (project.OwnerID != currentUser.Id && (currentMember == null || currentMember.Role != ProjectRole.Admin))
            return Forbid();

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAvailableUsers(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.ID == projectId);

        if (project == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return Unauthorized();

        // Проверяем права на просмотр доступных пользователей
        var currentMember = project.Members.FirstOrDefault(m => m.UserID == currentUser.Id);
        if (project.OwnerID != currentUser.Id && (currentMember == null || currentMember.Role != ProjectRole.Admin))
            return Forbid();

        // Получаем список всех пользователей, которые еще не являются участниками проекта
        var existingMemberIds = project.Members.Select(m => m.UserID).ToList();
        existingMemberIds.Add(project.OwnerID); // Добавляем владельца проекта

        var availableUsers = await _userManager.Users
            .Where(u => !existingMemberIds.Contains(u.Id))
            .Select(u => new UserDTO
            {
                ID = u.Id,
                UserName = u.UserName ?? string.Empty,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty
            })
            .ToListAsync();

        return Ok(availableUsers);
    }
} 