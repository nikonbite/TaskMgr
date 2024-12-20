using System.Net.Http.Json;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Services;

public class TaskService
{
    private readonly HttpClient _httpClient;

    public TaskService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TaskDTO>> GetProjectTasks(int projectId)
    {
        var response = await _httpClient.GetAsync($"api/tasks/project/{projectId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<TaskDTO>>() ?? Array.Empty<TaskDTO>();
    }

    public async Task<IEnumerable<TaskDTO>> GetMyTasks()
    {
        var response = await _httpClient.GetAsync("api/tasks/my");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<TaskDTO>>() ?? Array.Empty<TaskDTO>();
    }

    public async Task<TaskDTO?> GetTask(int id)
    {
        var response = await _httpClient.GetAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskDTO>();
    }

    public async Task<TaskDTO?> CreateTask(CreateTaskDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/tasks", model);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TaskDTO>();
    }

    public async Task UpdateTask(int id, UpdateTaskDTO model)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/tasks/{id}", model);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTask(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/tasks/{id}");
        response.EnsureSuccessStatusCode();
    }
} 