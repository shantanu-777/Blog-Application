using System.Net.Http.Json;
using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public class AdminService : IAdminService
{
    private readonly HttpClient _httpClient;

    public AdminService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AdminDashboardMetrics?> GetDashboardMetricsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AdminDashboardMetrics>("api/admin/dashboard");
        }
        catch
        {
            return null;
        }
    }

    public async Task<PagedResult<BlogPost>> GetModerationPostsAsync(AdminPostFilter filter)
    {
        var query = BuildPostFilterQuery(filter);
        try
        {
            return await _httpClient.GetFromJsonAsync<PagedResult<BlogPost>>(query)
                   ?? new PagedResult<BlogPost> { Items = new List<BlogPost>() };
        }
        catch
        {
            return new PagedResult<BlogPost> { Items = new List<BlogPost>() };
        }
    }

    public async Task<bool> UpdatePostStatusAsync(string postId, PostStatus status, string? reviewNotes = null)
    {
        var payload = new UpdatePostModerationRequest
        {
            Status = status,
            ReviewNotes = reviewNotes
        };

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/moderation/posts/{postId}", payload);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PagedResult<CommentModerationSummary>> GetModerationCommentsAsync(AdminCommentFilter filter)
    {
        var query = BuildCommentFilterQuery(filter);
        try
        {
            return await _httpClient.GetFromJsonAsync<PagedResult<CommentModerationSummary>>(query)
                   ?? new PagedResult<CommentModerationSummary> { Items = new List<CommentModerationSummary>() };
        }
        catch
        {
            return new PagedResult<CommentModerationSummary> { Items = new List<CommentModerationSummary>() };
        }
    }

    public async Task<bool> ModerateCommentAsync(string commentId, CommentModerationRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/admin/moderation/comments/{commentId}", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PagedResult<UserAdminSummary>> GetUsersAsync(AdminUserFilter filter)
    {
        var query = BuildUserFilterQuery(filter);
        try
        {
            return await _httpClient.GetFromJsonAsync<PagedResult<UserAdminSummary>>(query)
                   ?? new PagedResult<UserAdminSummary> { Items = new List<UserAdminSummary>() };
        }
        catch
        {
            return new PagedResult<UserAdminSummary> { Items = new List<UserAdminSummary>() };
        }
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, UserRole role)
    {
        try
        {
            var payload = new UpdateUserRoleRequest { Role = role };
            var response = await _httpClient.PutAsJsonAsync($"api/admin/users/{userId}/role", payload);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateUserStatusAsync(string userId, bool isActive)
    {
        try
        {
            var payload = new UpdateUserStatusRequest { IsActive = isActive };
            var response = await _httpClient.PutAsJsonAsync($"api/admin/users/{userId}/status", payload);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<AdminAnalyticsResponse?> GetAnalyticsAsync(int rangeDays = 30)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AdminAnalyticsResponse>($"api/admin/analytics/posts?rangeDays={rangeDays}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<SiteSettings?> GetSiteSettingsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SiteSettings>("api/admin/settings");
        }
        catch
        {
            return null;
        }
    }

    public async Task<SiteSettings?> UpdateSiteSettingsAsync(SiteSettings settings)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/admin/settings", settings);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SiteSettings>();
        }
        catch
        {
            return null;
        }
    }

    private static string BuildPostFilterQuery(AdminPostFilter filter)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Max(1, filter.PageSize);
        var query = $"api/admin/moderation/posts?page={page}&pageSize={pageSize}";

        if (filter.Status.HasValue)
            query += $"&status={(int)filter.Status.Value}";
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query += $"&search={Uri.EscapeDataString(filter.Search)}";

        return query;
    }

    private static string BuildCommentFilterQuery(AdminCommentFilter filter)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Max(1, filter.PageSize);
        var query = $"api/admin/moderation/comments?page={page}&pageSize={pageSize}";

        if (filter.FlaggedOnly.HasValue)
            query += $"&flaggedOnly={filter.FlaggedOnly.Value}";
        if (filter.PendingApprovalOnly.HasValue)
            query += $"&pendingApprovalOnly={filter.PendingApprovalOnly.Value}";
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query += $"&search={Uri.EscapeDataString(filter.Search)}";

        return query;
    }

    private static string BuildUserFilterQuery(AdminUserFilter filter)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Max(1, filter.PageSize);
        var query = $"api/admin/users?page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query += $"&search={Uri.EscapeDataString(filter.Search)}";
        if (filter.Role.HasValue)
            query += $"&role={(int)filter.Role.Value}";
        if (filter.IsActive.HasValue)
            query += $"&isActive={filter.IsActive.Value}";

        return query;
    }
}

