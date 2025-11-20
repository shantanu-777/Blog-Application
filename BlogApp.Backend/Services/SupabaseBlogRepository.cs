using System.Text;
using System.Text.Json;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface ISupabaseBlogRepository
{
    Task<BlogPost> InsertPostAsync(BlogPost post);
    Task<BlogPost?> GetPostByIdAsync(string id);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<List<BlogPost>> GetPostsAsync(GetPostsRequest request);
    Task<BlogPost?> UpdatePostAsync(string id, BlogPost post);
    Task<bool> DeletePostAsync(string id);

    Task<List<Comment>> GetCommentsAsync(string postId);
    Task<Comment?> GetCommentByIdAsync(string id);
    Task<Comment> InsertCommentAsync(Comment comment);
    Task<Comment?> UpdateCommentAsync(string id, string content);
    Task<bool> DeleteCommentAsync(string id);

    Task<bool> InsertLikeAsync(string postId, string userId);
    Task<bool> DeleteLikeAsync(string postId, string userId);
    Task<bool> HasUserLikedAsync(string postId, string userId);
    Task<int> CountLikesAsync(string postId);
}

public class SupabaseBlogRepository : ISupabaseBlogRepository
{
    private readonly SupabaseRestClient _client;

    public SupabaseBlogRepository(SupabaseRestClient client)
    {
        _client = client;
    }

    public async Task<BlogPost> InsertPostAsync(BlogPost post)
    {
        var dto = post.ToDto();
        var created = await _client.PostAsync<SupabasePostDto>("posts", dto) ?? dto;
        return created.ToModel();
    }

    public async Task<BlogPost?> GetPostByIdAsync(string id)
    {
        var posts = await _client.GetAsync<SupabasePostDto>($"posts?select=*&id=eq.{Uri.EscapeDataString(id)}&limit=1");
        return posts.FirstOrDefault()?.ToModel();
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        var posts = await _client.GetAsync<SupabasePostDto>($"posts?select=*&slug=eq.{Uri.EscapeDataString(slug)}&limit=1");
        return posts.FirstOrDefault()?.ToModel();
    }

    public async Task<List<BlogPost>> GetPostsAsync(GetPostsRequest request)
    {
        var builder = new StringBuilder("posts?select=*");
        builder.Append($"&order={request.SortBy ?? "created_at"}.{request.SortOrder ?? "desc"}");
        builder.Append($"&limit={request.PageSize}");
        var offset = (request.Page - 1) * request.PageSize;
        builder.Append($"&offset={offset}");

        if (!string.IsNullOrEmpty(request.AuthorId))
            builder.Append($"&author_id=eq.{Uri.EscapeDataString(request.AuthorId)}");

        if (!string.IsNullOrEmpty(request.Tag))
            builder.Append($"&tags=cs.{{\"{Uri.EscapeDataString(request.Tag)}\"}}");

        if (!string.IsNullOrEmpty(request.Category))
            builder.Append($"&categories=cs.{{\"{Uri.EscapeDataString(request.Category)}\"}}");

        if (request.Status.HasValue)
            builder.Append($"&status=eq.{(int)request.Status.Value}");

        if (!string.IsNullOrEmpty(request.SearchQuery))
        {
            var search = request.SearchQuery.Replace(" ", "%");
            builder.Append($"&or=(title.ilike.*{Uri.EscapeDataString(search)}*,excerpt.ilike.*{Uri.EscapeDataString(search)}*,content.ilike.*{Uri.EscapeDataString(search)}*)");
        }

        var posts = await _client.GetAsync<SupabasePostDto>(builder.ToString());
        return posts.Select(p => p.ToModel()).ToList();
    }

    public async Task<BlogPost?> UpdatePostAsync(string id, BlogPost post)
    {
        var dto = post.ToDto();
        var updated = await _client.PatchAsync<SupabasePostDto>($"posts?id=eq.{Uri.EscapeDataString(id)}", dto);
        return updated?.ToModel();
    }

    public async Task<bool> DeletePostAsync(string id)
    {
        await _client.DeleteAsync($"comments?post_id=eq.{Uri.EscapeDataString(id)}");
        await _client.DeleteAsync($"post_likes?post_id=eq.{Uri.EscapeDataString(id)}");
        await _client.DeleteAsync($"posts?id=eq.{Uri.EscapeDataString(id)}");
        return true;
    }

    public async Task<List<Comment>> GetCommentsAsync(string postId)
    {
        var comments = await _client.GetAsync<SupabaseCommentDto>(
            $"comments?select=*&post_id=eq.{Uri.EscapeDataString(postId)}&is_approved=eq.true&is_deleted=eq.false&order=created_at.asc");
        return comments.Select(c => c.ToModel()).ToList();
    }

    public async Task<Comment?> GetCommentByIdAsync(string id)
    {
        var comments = await _client.GetAsync<SupabaseCommentDto>($"comments?select=*&id=eq.{Uri.EscapeDataString(id)}&limit=1");
        return comments.FirstOrDefault()?.ToModel();
    }

    public async Task<Comment> InsertCommentAsync(Comment comment)
    {
        var dto = comment.ToDto();
        var created = await _client.PostAsync<SupabaseCommentDto>("comments", dto) ?? dto;
        return created.ToModel();
    }

    public async Task<Comment?> UpdateCommentAsync(string id, string content)
    {
        var payload = new { content, updated_at = DateTime.UtcNow };
        var updated = await _client.PatchAsync<SupabaseCommentDto>($"comments?id=eq.{Uri.EscapeDataString(id)}", payload);
        return updated?.ToModel();
    }

    public async Task<bool> DeleteCommentAsync(string id)
    {
        await _client.DeleteAsync($"comments?id=eq.{Uri.EscapeDataString(id)}");
        return true;
    }

    public async Task<bool> InsertLikeAsync(string postId, string userId)
    {
        var payload = new SupabaseLikeDto
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _client.PostAsync<SupabaseLikeDto>("post_likes", payload);
        return true;
    }

    public async Task<bool> DeleteLikeAsync(string postId, string userId)
    {
        await _client.DeleteAsync($"post_likes?post_id=eq.{Uri.EscapeDataString(postId)}&user_id=eq.{Uri.EscapeDataString(userId)}");
        return true;
    }

    public async Task<bool> HasUserLikedAsync(string postId, string userId)
    {
        var likes = await _client.GetAsync<SupabaseLikeDto>($"post_likes?select=id&post_id=eq.{Uri.EscapeDataString(postId)}&user_id=eq.{Uri.EscapeDataString(userId)}&limit=1");
        return likes.Any();
    }

    public async Task<int> CountLikesAsync(string postId)
    {
        var likes = await _client.GetAsync<SupabaseLikeDto>($"post_likes?select=id&post_id=eq.{Uri.EscapeDataString(postId)}");
        return likes.Count;
    }
}

