using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlogApp.Frontend.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly IAuthService _authService;

    public CustomAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
        _authService.AuthenticationStateChanged += HandleAuthenticationStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _authService.GetCurrentUserAsync();

        if (user == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("DisplayName", user.DisplayName ?? user.Username)
        };

        var identity = new ClaimsIdentity(claims, "Supabase");
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationState(principal);
    }

    private void HandleAuthenticationStateChanged(bool _)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void Dispose()
    {
        _authService.AuthenticationStateChanged -= HandleAuthenticationStateChanged;
    }
}
