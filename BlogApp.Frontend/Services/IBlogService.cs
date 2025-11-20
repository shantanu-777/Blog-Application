using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public interface IBlogService
{
    Task<List<BlogPost>> GetPostsAsync(int page = 1, int pageSize = 10, string? authorId = null, string? tag = null, string? category = null, string? searchQuery = null);
    Task<BlogPost?> GetPostAsync(string id);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<BlogPost> CreatePostAsync(CreatePostRequest request);
    Task<BlogPost> UpdatePostAsync(string id, UpdatePostRequest request);
    Task<bool> DeletePostAsync(string id);
    Task<bool> LikePostAsync(string postId);
    Task<bool> UnlikePostAsync(string postId);
    Task<List<Comment>> GetCommentsAsync(string postId);
    Task<Comment> AddCommentAsync(string postId, string content, string? parentCommentId = null);
    Task<Comment> UpdateCommentAsync(string commentId, string content);
    Task<bool> DeleteCommentAsync(string commentId);
    Task<List<BlogPost>> GetTrendingPostsAsync(int count = 10);
    Task<List<BlogPost>> GetRecommendedPostsAsync(int count = 10);
    Task<List<BlogPost>> SearchPostsAsync(string query, int page = 1, int pageSize = 10);
    Task<List<BlogPost>> GetPostsByAuthorAsync(string authorId);
    Task<List<BlogPost>> GetRelatedPostsAsync(string postId, int count = 3);
    Task<bool> IsPostLikedAsync(string postId);
}

