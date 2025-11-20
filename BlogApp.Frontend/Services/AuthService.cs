using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Blazored.LocalStorage;
using BlogApp.Shared.Models;
using Microsoft.Extensions.Configuration;

namespace BlogApp.Frontend.Services;

public class AuthService : IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _supabaseClient;
    private readonly HashSet<string> _adminAllowList;
    private User? _currentUser;

    public event Action<bool>? AuthenticationStateChanged;

    public AuthService(ILocalStorageService localStorage, IConfiguration configuration)
    {
        _localStorage = localStorage;
        _configuration = configuration;

        var supabaseUrl = _configuration["Supabase:Url"]
            ?? throw new InvalidOperationException("Supabase:Url not configured.");

        _supabaseClient = new HttpClient
        {
            BaseAddress = new Uri(supabaseUrl)
        };

        var allowList = _configuration.GetSection("Admin:AllowedEmails").Get<string[]>() ?? Array.Empty<string>();
        _adminAllowList = allowList
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var payload = new { email, password };
            var response = await SendSupabaseRequestAsync<SupabaseAuthResponse>(
            HttpMethod.Post,
            "auth/v1/token?grant_type=password",
            payload,
                useAccessToken: false);

        if (response == null || string.IsNullOrEmpty(response.AccessToken))
            return false;

        await StoreSessionAsync(response);
        return true;
    }

    public async Task<bool> RegisterAsync(string email, string password, string username, string? displayName = null)
    {
        var payload = new
        {
            email,
            password,
            data = new
            {
                username,
                display_name = displayName ?? username
            }
        };

        var response = await SendSupabaseRequestAsync<SupabaseAuthResponse>(
            HttpMethod.Post,
            "auth/v1/signup",
            payload,
            useAccessToken: false);

        if (response == null || string.IsNullOrEmpty(response.AccessToken))
            return false;

        await StoreSessionAsync(response);
        return true;
    }

    public async Task LogoutAsync()
    {
        try
        {
            var accessToken = await _localStorage.GetItemAsync<string>("authToken");
            var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

            if (!string.IsNullOrEmpty(accessToken))
            {
                var payload = new { refresh_token = refreshToken };
                await SendSupabaseRequestAsync<object>(
                    HttpMethod.Post,
                    "auth/v1/logout",
                    payload,
                    tokenOverride: accessToken,
                    suppressErrors: true);
            }
        }
        catch
        {
            // Ignore Supabase logout errors, we'll still clear local state.
        }
        finally
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");
            _currentUser = null;
            AuthenticationStateChanged?.Invoke(false);
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var user = await GetCurrentUserAsync();
        return user != null;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrEmpty(token))
            return null;

        var response = await SendSupabaseRequestAsync<SupabaseUser>(
            HttpMethod.Get,
            "auth/v1/user",
            content: null,
            tokenOverride: token,
            suppressErrors: true);

        if (response == null)
        return null;

        _currentUser = MapSupabaseUser(response);
        ApplyAdminOverrides(_currentUser);
        return _currentUser;
    }

    private async Task StoreSessionAsync(SupabaseAuthResponse response)
    {
        if (!string.IsNullOrEmpty(response.AccessToken))
        {
            await _localStorage.SetItemAsync("authToken", response.AccessToken);
        }

        if (!string.IsNullOrEmpty(response.RefreshToken))
        {
            await _localStorage.SetItemAsync("refreshToken", response.RefreshToken);
        }

        if (response.User != null)
        {
            _currentUser = MapSupabaseUser(response.User);
            ApplyAdminOverrides(_currentUser);
        }

        AuthenticationStateChanged?.Invoke(true);
    }

    private async Task<T?> SendSupabaseRequestAsync<T>(
        HttpMethod method,
        string relativePath,
        object? content = null,
        string? tokenOverride = null,
        bool useAccessToken = true,
        bool suppressErrors = false)
    {
        var anonKey = _configuration["Supabase:AnonKey"]
            ?? throw new InvalidOperationException("Supabase:AnonKey not configured.");

        var requestUri = BuildRequestUri(relativePath, anonKey);
        var request = new HttpRequestMessage(method, requestUri);

        request.Headers.Add("apikey", anonKey);
        request.Headers.Add("X-Client-Info", "BlogApp/1.0");

        var token = tokenOverride;
        if (string.IsNullOrEmpty(token))
        {
            token = useAccessToken ? await _localStorage.GetItemAsync<string>("authToken") : anonKey;
        }

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (content != null)
        {
            request.Content = JsonContent.Create(content);
        }

        var response = await _supabaseClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            if (suppressErrors)
            {
                return default;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Supabase request failed ({response.StatusCode}): {error}");
        }

        if (response.Content == null)
            return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    private static string BuildRequestUri(string relativePath, string anonKey)
    {
        var separator = relativePath.Contains('?') ? '&' : '?';
        return $"{relativePath}{separator}apikey={Uri.EscapeDataString(anonKey)}";
    }

    private static User MapSupabaseUser(SupabaseUser supabaseUser)
    {
        var metadata = supabaseUser.UserMetadata ?? new Dictionary<string, object?>();
        metadata.TryGetValue("username", out var usernameValue);
        metadata.TryGetValue("display_name", out var displayNameValue);
        metadata.TryGetValue("avatar_url", out var avatarValue);
        metadata.TryGetValue("bio", out var bioValue);
        metadata.TryGetValue("role", out var roleValue);
        if (roleValue == null && metadata.TryGetValue("app_role", out var appRoleValue))
        {
            roleValue = appRoleValue;
        }

        var role = UserRole.Reader;
        if (roleValue != null && Enum.TryParse(roleValue.ToString(), true, out UserRole parsedRole))
        {
            role = parsedRole;
        }

        return new User
        {
            Id = supabaseUser.Id ?? string.Empty,
            Email = supabaseUser.Email ?? string.Empty,
            Username = usernameValue?.ToString() ?? supabaseUser.Email?.Split('@').FirstOrDefault() ?? string.Empty,
            DisplayName = displayNameValue?.ToString(),
            AvatarUrl = avatarValue?.ToString(),
            Bio = bioValue?.ToString(),
            IsEmailVerified = supabaseUser.EmailConfirmedAt.HasValue,
            CreatedAt = supabaseUser.CreatedAt ?? DateTime.UtcNow,
            LastLoginAt = supabaseUser.LastSignInAt,
            Role = role
        };
    }

    private void ApplyAdminOverrides(User? user)
    {
        if (user == null)
            return;

        if (_adminAllowList.Contains(user.Email))
        {
            user.Role = UserRole.Admin;
        }
    }

    private class SupabaseAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("user")]
        public SupabaseUser? User { get; set; }
    }

    private class SupabaseUser
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("last_sign_in_at")]
        public DateTime? LastSignInAt { get; set; }

        [JsonPropertyName("email_confirmed_at")]
        public DateTime? EmailConfirmedAt { get; set; }

        [JsonPropertyName("user_metadata")]
        public Dictionary<string, object?>? UserMetadata { get; set; }
    }
}
