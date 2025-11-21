using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface IBlogService
{
    Task<BlogPost> CreatePostAsync(CreatePostRequest request);
    Task<BlogPost?> GetPostAsync(string id);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<List<BlogPost>> GetPostsAsync(GetPostsRequest request);
    Task<BlogPost> UpdatePostAsync(string id, UpdatePostRequest request);
    Task<bool> DeletePostAsync(string id);
    Task<bool> LikePostAsync(string postId, string userId);
    Task<bool> UnlikePostAsync(string postId, string userId);
    Task<List<Comment>> GetCommentsAsync(string postId);
    Task<Comment> AddCommentAsync(AddCommentRequest request);
    Task<Comment> UpdateCommentAsync(string id, UpdateCommentRequest request);
    Task<bool> DeleteCommentAsync(string id);
    Task<List<BlogPost>> GetTrendingPostsAsync(int count = 10);
    Task<List<BlogPost>> GetRecommendedPostsAsync(string userId, int count = 10);
    Task<List<BlogPost>> SearchPostsAsync(string query, int page = 1, int pageSize = 10);
    Task<List<BlogPost>> GetRelatedPostsAsync(string postId, int count = 3);
    Task<bool> IsPostLikedAsync(string postId, string userId);
    Task<List<ArchiveEntry>> GetArchiveEntriesAsync();
    Task<List<BlogPost>> GetPostsByArchiveAsync(int year, int? month = null, int page = 1, int pageSize = 10);
}


public class GetPostsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? AuthorId { get; set; }
    public string? Tag { get; set; }
    public string? Category { get; set; }
    public PostStatus? Status { get; set; }
    public string? SearchQuery { get; set; }
    public string? SortBy { get; set; } = "created_at";
    public string? SortOrder { get; set; } = "desc";
}

public class AddCommentRequest
{
    public string PostId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
}

public class UpdateCommentRequest
{
    public string Content { get; set; } = string.Empty;
}

public class ArchiveEntry
{
    public int Year { get; set; }
    public int? Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int PostCount { get; set; }
}
