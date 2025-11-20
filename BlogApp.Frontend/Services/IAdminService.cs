using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public interface IAdminService
{
    Task<AdminDashboardMetrics?> GetDashboardMetricsAsync();
    Task<PagedResult<BlogPost>> GetModerationPostsAsync(AdminPostFilter filter);
    Task<bool> UpdatePostStatusAsync(string postId, PostStatus status, string? reviewNotes = null);
    Task<PagedResult<CommentModerationSummary>> GetModerationCommentsAsync(AdminCommentFilter filter);
    Task<bool> ModerateCommentAsync(string commentId, CommentModerationRequest request);
    Task<PagedResult<UserAdminSummary>> GetUsersAsync(AdminUserFilter filter);
    Task<bool> UpdateUserRoleAsync(string userId, UserRole role);
    Task<bool> UpdateUserStatusAsync(string userId, bool isActive);
    Task<AdminAnalyticsResponse?> GetAnalyticsAsync(int rangeDays = 30);
    Task<SiteSettings?> GetSiteSettingsAsync();
    Task<SiteSettings?> UpdateSiteSettingsAsync(SiteSettings settings);
}

