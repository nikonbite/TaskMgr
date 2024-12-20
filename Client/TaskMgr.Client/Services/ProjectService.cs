using System.Net.Http.Json;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Services;

public class ProjectService
{
    private readonly HttpClient _httpClient;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<ProjectDTO>> GetProjects()
    {
        var response = await _httpClient.GetAsync("api/projects");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<ProjectDTO>>() ?? Array.Empty<ProjectDTO>();
    }

    public async Task<ProjectDTO?> GetProject(int id)
    {
        var response = await _httpClient.GetAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDTO>();
    }

    public async Task<ProjectDTO?> CreateProject(CreateProjectDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/projects", model);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDTO>();
    }

    public async Task UpdateProject(int id, UpdateProjectDTO model)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/projects/{id}", model);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteProject(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }
} 