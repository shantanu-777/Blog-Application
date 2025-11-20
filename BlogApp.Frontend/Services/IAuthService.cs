using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string password, string username, string? displayName = null);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<User?> GetCurrentUserAsync();
    event Action<bool>? AuthenticationStateChanged;
}
