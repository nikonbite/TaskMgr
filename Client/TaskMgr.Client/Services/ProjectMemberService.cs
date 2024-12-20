using System.Net.Http.Json;
using TaskMgr.Server.Models;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Services;

public class ProjectMemberService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ProjectMemberService> _logger;

    public ProjectMemberService(IHttpClientFactory httpClientFactory, ILogger<ProjectMemberService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectMemberDTO>> GetMembers(int projectId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/projects/{projectId}/members");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<ProjectMemberDTO>>() ?? Array.Empty<ProjectMemberDTO>();
    }

    public async Task<IEnumerable<UserDTO>> GetAvailableUsers(int projectId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/projects/{projectId}/members/available");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>() ?? Array.Empty<UserDTO>();
    }

    public async Task<ProjectMemberDTO> AddMember(int projectId, AddProjectMemberDTO dto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PostAsJsonAsync($"api/projects/{projectId}/members", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectMemberDTO>() ?? throw new InvalidOperationException();
    }

    public async Task UpdateMember(int projectId, int memberId, UpdateProjectMemberDTO dto)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.PutAsJsonAsync($"api/projects/{projectId}/members/{memberId}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveMember(int projectId, int memberId)
    {
        var client = _httpClientFactory.CreateClient("API");
        var response = await client.DeleteAsync($"api/projects/{projectId}/members/{memberId}");
        response.EnsureSuccessStatusCode();
    }
} 