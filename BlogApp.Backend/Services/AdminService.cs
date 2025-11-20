using System.Globalization;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface IAdminService
{
    Task<AdminDashboardMetrics> GetDashboardMetricsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<BlogPost>> GetPostModerationQueueAsync(AdminPostFilter filter, CancellationToken cancellationToken = default);
    Task<bool> UpdatePostStatusAsync(string postId, UpdatePostModerationRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<CommentModerationSummary>> GetCommentModerationQueueAsync(AdminCommentFilter filter, CancellationToken cancellationToken = default);
    Task<bool> ModerateCommentAsync(string commentId, CommentModerationRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<UserAdminSummary>> GetUsersAsync(AdminUserFilter filter, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserRoleAsync(string userId, UserRole role, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
    Task<AdminAnalyticsResponse> GetPostAnalyticsAsync(int rangeDays = 30, CancellationToken cancellationToken = default);
    Task<SiteSettings> GetSiteSettingsAsync(CancellationToken cancellationToken = default);
    Task<SiteSettings> UpdateSiteSettingsAsync(SiteSettings settings, CancellationToken cancellationToken = default);
}

public class AdminService : IAdminService
{
    private readonly SupabaseRestClient _client;
    private readonly ISupabaseBlogRepository _repository;
    private readonly ISiteSettingsService _siteSettingsService;
    private readonly IUserService _userService;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        SupabaseRestClient client,
        ISupabaseBlogRepository repository,
        ISiteSettingsService siteSettingsService,
        IUserService userService,
        ILogger<AdminService> logger)
    {
        _client = client;
        _repository = repository;
        _siteSettingsService = siteSettingsService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<AdminDashboardMetrics> GetDashboardMetricsAsync(CancellationToken cancellationToken = default)
    {
        var (postDtos, totalPosts) = await _client.GetWithCountAsync<SupabasePostDto>(
            "posts?select=*&order=created_at.desc&limit=500",
            cancellationToken);
        var (commentDtos, _) = await _client.GetWithCountAsync<SupabaseCommentDto>(
            "comments?select=*&order=created_at.desc&limit=500",
            cancellationToken);

        var posts = postDtos.Select(p => p.ToModel()).ToList();
        var comments = commentDtos.Select(c => c.ToModel()).ToList();
        var users = await _userService.GetAllUsersAsync();

        var metrics = new AdminDashboardMetrics
        {
            TotalPosts = totalPosts ?? posts.Count,
            DraftPosts = posts.Count(p => p.Status == PostStatus.Draft),
            PendingPosts = posts.Count(p => p.Status == PostStatus.PendingReview),
            PublishedPosts = posts.Count(p => p.Status == PostStatus.Published),
            ScheduledPosts = posts.Count(p => p.Status == PostStatus.Published && p.ScheduledAt.HasValue),
            TotalViews = posts.Sum(p => p.ViewsCount),
            TotalLikes = posts.Sum(p => p.LikesCount),
            TotalComments = posts.Sum(p => p.CommentsCount),
            ActiveAuthors = posts.Select(p => p.AuthorId).Distinct().Count(),
            PendingComments = comments.Count(c => !c.IsApproved && !c.IsDeleted),
            FlaggedComments = comments.Count(c => c.IsFlagged && !c.IsDeleted),
            NewUsersThisWeek = users.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-7)),
            ActiveReaders = users.Count(u => u.Role == UserRole.Reader && u.IsActive)
        };

        metrics.EngagementRate = metrics.TotalViews == 0
            ? 0
            : Math.Round((double)(metrics.TotalLikes + metrics.TotalComments) / metrics.TotalViews, 3);

        metrics.PostTrends = posts
            .GroupBy(p => (p.CreatedAt == default ? DateTime.UtcNow : p.CreatedAt).Date)
            .OrderBy(g => g.Key)
            .TakeLast(14)
            .Select(g => new PostAnalyticsPoint
            {
                Date = g.Key,
                Posts = g.Count(),
                Views = g.Sum(p => p.ViewsCount)
            })
            .ToList();

        metrics.TopPosts = posts
            .OrderByDescending(p => p.ViewsCount + p.LikesCount + p.CommentsCount)
            .Take(5)
            .Select(p => new PostEngagementMetric
            {
                PostId = p.Id,
                Title = p.Title,
                Views = p.ViewsCount,
                Likes = p.LikesCount,
                Comments = p.CommentsCount
            })
            .ToList();

        metrics.RecentFlags = comments
            .Where(c => c.IsFlagged || (!c.IsApproved && !c.IsDeleted))
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new CommentModerationSummary
            {
                Id = c.Id,
                AuthorId = c.AuthorId,
                PostId = c.PostId,
                Content = c.Content,
                IsFlagged = c.IsFlagged,
                IsApproved = c.IsApproved,
                FlagReason = c.FlagReason,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        metrics.LatestMembers = users
            .OrderByDescending(u => u.CreatedAt)
            .Take(5)
            .Select(u => new UserAdminSummary
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                PostsCount = u.PostsCount
            })
            .ToList();

        return metrics;
    }

    public async Task<PagedResult<BlogPost>> GetPostModerationQueueAsync(AdminPostFilter filter, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);
        var offset = (page - 1) * pageSize;
        var queryParts = new List<string>
        {
            "select=*",
            $"order=updated_at.desc",
            $"limit={pageSize}",
            $"offset={offset}"
        };

        if (filter.Status.HasValue)
        {
            queryParts.Add($"status=eq.{(int)filter.Status.Value}");
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var escaped = Uri.EscapeDataString(filter.Search.Replace(" ", "%"));
            queryParts.Add($"or=(title.ilike.*{escaped}*,excerpt.ilike.*{escaped}*)");
        }

        var (dtos, total) = await _client.GetWithCountAsync<SupabasePostDto>($"posts?{string.Join("&", queryParts)}", cancellationToken);
        var posts = dtos.Select(d => d.ToModel()).ToList();

        return new PagedResult<BlogPost>
        {
            Items = posts,
            Page = page,
            PageSize = pageSize,
            Total = total ?? posts.Count
        };
    }

    public async Task<bool> UpdatePostStatusAsync(string postId, UpdatePostModerationRequest request, CancellationToken cancellationToken = default)
    {
        var post = await _repository.GetPostByIdAsync(postId);
        if (post == null)
            return false;

        post.Status = request.Status;
        post.ReviewNotes = request.ReviewNotes;
        post.UpdatedAt = DateTime.UtcNow;

        if (request.Status == PostStatus.Published)
        {
            post.PublishedAt ??= DateTime.UtcNow;
        }

        if (request.Status == PostStatus.Draft)
        {
            post.PublishedAt = null;
        }

        await _repository.UpdatePostAsync(postId, post);
        _logger.LogInformation("Updated post {PostId} status to {Status}", postId, request.Status);
        return true;
    }

    public async Task<PagedResult<CommentModerationSummary>> GetCommentModerationQueueAsync(AdminCommentFilter filter, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 50);
        var offset = (page - 1) * pageSize;

        var queryParts = new List<string>
        {
            "select=*",
            $"order=created_at.desc",
            $"limit={pageSize}",
            $"offset={offset}"
        };

        var conditions = new List<string>();
        if (filter.FlaggedOnly == true)
            conditions.Add("is_flagged=eq.true");
        if (filter.PendingApprovalOnly == true)
            conditions.Add("is_approved=eq.false");
        if (!conditions.Any())
            conditions.Add("or=(is_flagged.eq.true,is_approved.eq.false)");

        foreach (var condition in conditions)
            queryParts.Add(condition);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var escaped = Uri.EscapeDataString(filter.Search.Replace(" ", "%"));
            queryParts.Add($"content=ilike.*{escaped}*");
        }

        var (dtos, total) = await _client.GetWithCountAsync<SupabaseCommentDto>($"comments?{string.Join("&", queryParts)}", cancellationToken);
        var summaries = dtos.Select(ToCommentSummary).ToList();

        return new PagedResult<CommentModerationSummary>
        {
            Items = summaries,
            Page = page,
            PageSize = pageSize,
            Total = total ?? summaries.Count
        };
    }

    public async Task<bool> ModerateCommentAsync(string commentId, CommentModerationRequest request, CancellationToken cancellationToken = default)
    {
        var comment = await _repository.GetCommentByIdAsync(commentId);
        if (comment == null)
            return false;

        var isApproved = request.Approve && !request.Delete;
        var isDeleted = request.Delete;
        var isFlagged = !isApproved && !isDeleted;
        var payload = new
        {
            is_approved = isApproved,
            is_deleted = isDeleted,
            is_flagged = isFlagged,
            flag_reason = request.Reason,
            updated_at = DateTime.UtcNow
        };

        await _client.PatchAsync<SupabaseCommentDto>($"comments?id=eq.{Uri.EscapeDataString(commentId)}", payload, cancellationToken);

        if ((isApproved && !comment.IsApproved) || (isDeleted && comment.IsApproved))
        {
            await SyncApprovedCommentsCountAsync(comment.PostId, cancellationToken);
        }

        _logger.LogInformation("Moderated comment {CommentId}. Approve={Approve} Delete={Delete}", commentId, request.Approve, request.Delete);
        return true;
    }

    public async Task<PagedResult<UserAdminSummary>> GetUsersAsync(AdminUserFilter filter, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUsersAsync(filter);
        return new PagedResult<UserAdminSummary>
        {
            Items = result.Items.Select(ToUserSummary).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            Total = result.Total
        };
    }

    public Task<bool> UpdateUserRoleAsync(string userId, UserRole role, CancellationToken cancellationToken = default)
        => _userService.UpdateUserRoleAsync(userId, role);

    public async Task<bool> UpdateUserStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
    {
        return isActive
            ? await _userService.ReactivateUserAsync(userId)
            : await _userService.DeactivateUserAsync(userId);
    }

    public async Task<AdminAnalyticsResponse> GetPostAnalyticsAsync(int rangeDays = 30, CancellationToken cancellationToken = default)
    {
        var from = DateTime.UtcNow.AddDays(-Math.Abs(rangeDays));
        var iso = Uri.EscapeDataString(from.ToString("o", CultureInfo.InvariantCulture));
        var (dtos, _) = await _client.GetWithCountAsync<SupabasePostDto>(
            $"posts?select=*&created_at=gte.{iso}&order=created_at.asc&limit=1000",
            cancellationToken);
        var posts = dtos.Select(d => d.ToModel()).ToList();

        var trend = posts
            .GroupBy(p => (p.CreatedAt == default ? DateTime.UtcNow : p.CreatedAt).Date)
            .Select(g => new PostAnalyticsPoint
            {
                Date = g.Key,
                Posts = g.Count(),
                Views = g.Sum(p => p.ViewsCount)
            })
            .OrderBy(p => p.Date)
            .ToList();

        var topPosts = posts
            .OrderByDescending(p => p.ViewsCount + p.LikesCount + p.CommentsCount)
            .Take(10)
            .Select(p => new PostEngagementMetric
            {
                PostId = p.Id,
                Title = p.Title,
                Views = p.ViewsCount,
                Likes = p.LikesCount,
                Comments = p.CommentsCount
            })
            .ToList();

        var categoryBreakdown = posts
            .SelectMany(p => p.Categories ?? new List<string>())
            .GroupBy(c => string.IsNullOrWhiteSpace(c) ? "Uncategorized" : c)
            .Select(g => new CategoryBreakdown
            {
                Category = g.Key,
                Posts = g.Count()
            })
            .OrderByDescending(c => c.Posts)
            .ToList();

        return new AdminAnalyticsResponse
        {
            ViewsTrend = trend,
            TopPosts = topPosts,
            CategoryDistribution = categoryBreakdown
        };
    }

    public Task<SiteSettings> GetSiteSettingsAsync(CancellationToken cancellationToken = default)
        => _siteSettingsService.GetSettingsAsync(cancellationToken);

    public Task<SiteSettings> UpdateSiteSettingsAsync(SiteSettings settings, CancellationToken cancellationToken = default)
        => _siteSettingsService.UpdateSettingsAsync(settings, cancellationToken);

    private static CommentModerationSummary ToCommentSummary(SupabaseCommentDto dto)
    {
        return new CommentModerationSummary
        {
            Id = dto.Id,
            AuthorId = dto.AuthorId,
            PostId = dto.PostId,
            Content = dto.Content,
            IsFlagged = dto.IsFlagged,
            IsApproved = dto.IsApproved,
            FlagReason = dto.FlagReason,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
        };
    }

    private static UserAdminSummary ToUserSummary(User user)
    {
        return new UserAdminSummary
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            PostsCount = user.PostsCount
        };
    }

    private async Task SyncApprovedCommentsCountAsync(string postId, CancellationToken cancellationToken)
    {
        var comments = await _client.GetAsync<SupabaseCommentDto>(
            $"comments?select=id&post_id=eq.{Uri.EscapeDataString(postId)}&is_approved=eq.true&is_deleted=eq.false",
            cancellationToken);
        var post = await _repository.GetPostByIdAsync(postId);
        if (post == null)
            return;

        post.CommentsCount = comments.Count;
        post.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdatePostAsync(postId, post);
    }
}

