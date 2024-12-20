using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using TaskMgr.Client.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:7001");
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddScoped<ProjectMemberService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "TaskMgr.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnSigningIn = async context =>
            {
                var principal = context.Principal;
                if (principal?.Identity is ClaimsIdentity identity)
                {
                    var accessToken = identity.FindFirst("access_token")?.Value;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(accessToken);
                        
                        // Добавляем все клеймы из JWT токена
                        foreach (var claim in token.Claims)
                        {
                            if (!identity.HasClaim(c => c.Type == claim.Type))
                            {
                                identity.AddClaim(new Claim(claim.Type, claim.Value));
                            }
                        }
                    }
                }
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();

// Делегированный обработчик для добавления токена к запросам
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
        var accessToken = _httpContextAccessor.HttpContext?.User?.FindFirst("access_token")?.Value;

        if (!string.IsNullOrEmpty(accessToken))
        {
            _logger.LogDebug("Добавление токена к запросу: {RequestUri}", request.RequestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        else
        {
            _logger.LogWarning("Токен не найден для запроса: {RequestUri}", request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
