namespace BlogApp.Shared.Models;

public class AdminDashboardMetrics
{
    public int TotalPosts { get; set; }
    public int DraftPosts { get; set; }
    public int PendingPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int ScheduledPosts { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int ActiveAuthors { get; set; }
    public int PendingComments { get; set; }
    public int FlaggedComments { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int ActiveReaders { get; set; }
    public double EngagementRate { get; set; }
    public List<PostAnalyticsPoint> PostTrends { get; set; } = new();
    public List<PostEngagementMetric> TopPosts { get; set; } = new();
    public List<CommentModerationSummary> RecentFlags { get; set; } = new();
    public List<UserAdminSummary> LatestMembers { get; set; } = new();
}

public class PostAnalyticsPoint
{
    public DateTime Date { get; set; }
    public int Posts { get; set; }
    public int Views { get; set; }
}

public class PostEngagementMetric
{
    public string PostId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
}

public class UpdatePostModerationRequest
{
    public PostStatus Status { get; set; } = PostStatus.PendingReview;
    public string? ReviewNotes { get; set; }
}

public class CommentModerationRequest
{
    public bool Approve { get; set; }
    public bool Delete { get; set; }
    public string? Reason { get; set; }
}

public class UpdateUserStatusRequest
{
    public bool IsActive { get; set; }
}

public class UpdateUserRoleRequest
{
    public UserRole Role { get; set; } = UserRole.Reader;
}

public class AdminUserFilter
{
    public string? Search { get; set; }
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AdminPostFilter
{
    public string? Search { get; set; }
    public PostStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class AdminCommentFilter
{
    public string? Search { get; set; }
    public bool? FlaggedOnly { get; set; }
    public bool? PendingApprovalOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class CommentModerationSummary
{
    public string Id { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsFlagged { get; set; }
    public bool IsApproved { get; set; }
    public string? FlagReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserAdminSummary
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int PostsCount { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

public class AdminAnalyticsResponse
{
    public List<PostAnalyticsPoint> ViewsTrend { get; set; } = new();
    public List<PostEngagementMetric> TopPosts { get; set; } = new();
    public List<CategoryBreakdown> CategoryDistribution { get; set; } = new();
}

public class CategoryBreakdown
{
    public string Category { get; set; } = string.Empty;
    public int Posts { get; set; }
}


