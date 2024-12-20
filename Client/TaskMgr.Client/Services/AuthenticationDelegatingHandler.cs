using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace TaskMgr.Client.Services;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationDelegatingHandler> _logger;

    public AuthenticationDelegatingHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationDelegatingHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var claims = httpContext.User.Claims;
            var tokenClaim = claims.FirstOrDefault(c => c.Type == "access_token");
            
            if (tokenClaim != null)
            {
                _logger.LogInformation("Добавление токена к запросу: {RequestUri}", request.RequestUri);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenClaim.Value);
            }
            else
            {
                _logger.LogWarning("Токен не найден в claims");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
} 