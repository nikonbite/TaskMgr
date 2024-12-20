using System.Net.Http.Json;
using TaskMgr.Server.Models.DTOs;

namespace TaskMgr.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthTokenDTO?> Login(LoginDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthTokenDTO>();
        }
        return null;
    }

    public async Task<AuthTokenDTO?> Register(RegisterDTO model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthTokenDTO>();
        }
        return null;
    }
} 