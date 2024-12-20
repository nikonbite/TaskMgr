using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMgr.Client.Services;
using TaskMgr.Server.Models;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Controllers;

[Authorize]
public class ProjectMembersController : Controller
{
    private readonly ProjectMemberService _memberService;

    public ProjectMembersController(ProjectMemberService memberService)
    {
        _memberService = memberService;
    }

    public async Task<IActionResult> Index(int projectId)
    {
        try
        {
            var members = await _memberService.GetMembers(projectId);
            ViewBag.ProjectId = projectId;
            return View(members);
        }
        catch (HttpRequestException)
        {
            return RedirectToAction("Index", "Projects");
        }
    }

    public async Task<IActionResult> Add(int projectId)
    {
        try
        {
            var availableUsers = await _memberService.GetAvailableUsers(projectId);
            ViewBag.ProjectId = projectId;
            ViewBag.Roles = Enum.GetValues<ProjectRole>();
            return View(availableUsers);
        }
        catch (HttpRequestException)
        {
            return RedirectToAction("Index", new { projectId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add(int projectId, string userId, ProjectRole role)
    {
        try
        {
            var dto = new AddProjectMemberDTO { UserID = userId, Role = role };
            await _memberService.AddMember(projectId, dto);
            return RedirectToAction("Index", new { projectId });
        }
        catch (HttpRequestException)
        {
            return RedirectToAction("Index", new { projectId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(int projectId, int memberId, ProjectRole role)
    {
        try
        {
            var dto = new UpdateProjectMemberDTO { Role = role };
            await _memberService.UpdateMember(projectId, memberId, dto);
            return RedirectToAction("Index", new { projectId });
        }
        catch (HttpRequestException)
        {
            return RedirectToAction("Index", new { projectId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int projectId, int memberId)
    {
        try
        {
            await _memberService.RemoveMember(projectId, memberId);
            return RedirectToAction("Index", new { projectId });
        }
        catch (HttpRequestException)
        {
            return RedirectToAction("Index", new { projectId });
        }
    }
} 