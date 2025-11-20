using BlogApp.Backend.Services;
using BlogApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardMetrics>> GetDashboard()
    {
        try
        {
            var metrics = await _adminService.GetDashboardMetricsAsync();
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load admin dashboard metrics");
            return StatusCode(500, new { message = "Unable to load dashboard metrics." });
        }
    }

    [HttpGet("moderation/posts")]
    public async Task<ActionResult<PagedResult<BlogPost>>> GetModerationPosts([FromQuery] AdminPostFilter filter)
    {
        try
        {
            filter ??= new AdminPostFilter();
            var posts = await _adminService.GetPostModerationQueueAsync(filter);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load pending posts");
            return StatusCode(500, new { message = "Unable to load pending posts." });
        }
    }

    [HttpPut("moderation/posts/{postId}")]
    public async Task<ActionResult> UpdatePostStatus(string postId, UpdatePostModerationRequest request)
    {
        try
        {
            var updated = await _adminService.UpdatePostStatusAsync(postId, request);
            return updated ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update post {PostId}", postId);
            return StatusCode(500, new { message = "Unable to update post status." });
        }
    }

    [HttpGet("moderation/comments")]
    public async Task<ActionResult<PagedResult<CommentModerationSummary>>> GetModerationComments([FromQuery] AdminCommentFilter filter)
    {
        try
        {
            filter ??= new AdminCommentFilter();
            var comments = await _adminService.GetCommentModerationQueueAsync(filter);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load flagged comments");
            return StatusCode(500, new { message = "Unable to load flagged comments." });
        }
    }

    [HttpGet("users")]
    public async Task<ActionResult<PagedResult<UserAdminSummary>>> GetUsers([FromQuery] AdminUserFilter filter)
    {
        try
        {
            filter ??= new AdminUserFilter();
            var result = await _adminService.GetUsersAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load users");
            return StatusCode(500, new { message = "Unable to load users." });
        }
    }

    [HttpPut("users/{id}/role")]
    public async Task<ActionResult> UpdateUserRole(string id, UpdateUserRoleRequest request)
    {
        try
        {
            var updated = await _adminService.UpdateUserRoleAsync(id, request.Role);
            return updated ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update role for user {UserId}", id);
            return StatusCode(500, new { message = "Unable to update user role." });
        }
    }

    [HttpPut("users/{id}/status")]
    public async Task<ActionResult> UpdateUserStatus(string id, UpdateUserStatusRequest request)
    {
        try
        {
            var updated = await _adminService.UpdateUserStatusAsync(id, request.IsActive);
            return updated ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for user {UserId}", id);
            return StatusCode(500, new { message = "Unable to update user status." });
        }
    }

    [HttpGet("analytics/posts")]
    public async Task<ActionResult<AdminAnalyticsResponse>> GetAnalytics([FromQuery] int rangeDays = 30)
    {
        try
        {
            var analytics = await _adminService.GetPostAnalyticsAsync(rangeDays);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load analytics");
            return StatusCode(500, new { message = "Unable to load analytics." });
        }
    }

    [HttpPut("moderation/comments/{commentId}")]
    public async Task<ActionResult> ModerateComment(string commentId, CommentModerationRequest request)
    {
        try
        {
            var result = await _adminService.ModerateCommentAsync(commentId, request);
            return result ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to moderate comment {CommentId}", commentId);
            return StatusCode(500, new { message = "Unable to moderate comment." });
        }
    }

    [HttpGet("settings")]
    public async Task<ActionResult<SiteSettings>> GetSettings()
    {
        try
        {
            var settings = await _adminService.GetSiteSettingsAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load site settings");
            return StatusCode(500, new { message = "Unable to load site settings." });
        }
    }

    [HttpPut("settings")]
    public async Task<ActionResult<SiteSettings>> UpdateSettings(SiteSettings request)
    {
        try
        {
            var updated = await _adminService.UpdateSiteSettingsAsync(request);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update site settings");
            return StatusCode(500, new { message = "Unable to update site settings." });
        }
    }
}

