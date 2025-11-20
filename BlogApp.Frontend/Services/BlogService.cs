using System.Net.Http;
using System.Net.Http.Json;
using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public class BlogService : IBlogService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public BlogService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<List<BlogPost>> GetPostsAsync(int page = 1, int pageSize = 10, string? authorId = null, string? tag = null, string? category = null, string? searchQuery = null)
    {
        try
        {
            var queryParams = new List<string>();
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");
            
            if (!string.IsNullOrEmpty(authorId))
                queryParams.Add($"authorId={authorId}");
            if (!string.IsNullOrEmpty(tag))
                queryParams.Add($"tag={tag}");
            if (!string.IsNullOrEmpty(category))
                queryParams.Add($"category={category}");
            if (!string.IsNullOrEmpty(searchQuery))
                queryParams.Add($"searchQuery={searchQuery}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>($"api/blog?{queryString}");
            return response ?? new List<BlogPost>();
        }
        catch (Exception)
        {
            return new List<BlogPost>();
        }
    }

    public async Task<BlogPost?> GetPostAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BlogPost>($"api/blog/{id}");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<BlogPost>($"api/blog/slug/{slug}");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<BlogPost> CreatePostAsync(CreatePostRequest request)
    {
        var user = await EnsureUserAsync();
        request.AuthorId = user.Id;

        var httpRequest = await CreateRequestAsync(HttpMethod.Post, "api/blog", request, includeUserHeader: true);
        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BlogPost>() ?? throw new Exception("Failed to create post");
    }

    public async Task<BlogPost> UpdatePostAsync(string id, UpdatePostRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/blog/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BlogPost>() ?? throw new Exception("Failed to update post");
    }

    public async Task<bool> DeletePostAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/blog/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> LikePostAsync(string postId)
    {
        try
        {
            var user = await EnsureUserAsync();
            var payload = new { userId = user.Id };
            var request = await CreateRequestAsync(HttpMethod.Post, $"api/blog/{postId}/like", payload, includeUserHeader: true);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UnlikePostAsync(string postId)
    {
        try
        {
            var user = await EnsureUserAsync();
            var payload = new { userId = user.Id };
            var request = await CreateRequestAsync(HttpMethod.Delete, $"api/blog/{postId}/like", payload, includeUserHeader: true);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Comment>> GetCommentsAsync(string postId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Comment>>($"api/blog/{postId}/comments");
            return response ?? new List<Comment>();
        }
        catch (Exception)
        {
            return new List<Comment>();
        }
    }

    public async Task<Comment> AddCommentAsync(string postId, string content, string? parentCommentId = null)
    {
        var user = await EnsureUserAsync();
        var payload = new { postId, content, parentCommentId, authorId = user.Id };
        var request = await CreateRequestAsync(HttpMethod.Post, $"api/blog/{postId}/comments", payload, includeUserHeader: true);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Comment>() ?? throw new Exception("Failed to add comment");
    }

    public async Task<Comment> UpdateCommentAsync(string commentId, string content)
    {
        var request = new { content };
        var response = await _httpClient.PutAsJsonAsync($"api/blog/comments/{commentId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Comment>() ?? throw new Exception("Failed to update comment");
    }

    public async Task<bool> DeleteCommentAsync(string commentId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/blog/comments/{commentId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<BlogPost>> GetTrendingPostsAsync(int count = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>($"api/blog/trending?count={count}");
            return response ?? new List<BlogPost>();
        }
        catch (Exception)
        {
            return new List<BlogPost>();
        }
    }

    public async Task<List<BlogPost>> GetRecommendedPostsAsync(int count = 10)
    {
        try
        {
            var user = await EnsureUserAsync();
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>($"api/blog/recommended?count={count}&userId={user.Id}");
            return response ?? new List<BlogPost>();
        }
        catch (Exception)
        {
            return new List<BlogPost>();
        }
    }

    public async Task<List<BlogPost>> SearchPostsAsync(string query, int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>($"api/blog/search?query={query}&page={page}&pageSize={pageSize}");
            return response ?? new List<BlogPost>();
        }
        catch (Exception)
        {
            return new List<BlogPost>();
        }
    }

    public async Task<List<BlogPost>> GetPostsByAuthorAsync(string authorId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>($"api/blog?authorId={authorId}");
            return response ?? new List<BlogPost>();
        }
        catch (Exception)
        {
            return new List<BlogPost>();
        }
    }

    public async Task<List<BlogPost>> GetRelatedPostsAsync(string postId, int count = 3)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>($"api/blog/{postId}/related?count={count}");
            return response ?? new List<BlogPost>();
        }
        catch (Exception)
        {
            return new List<BlogPost>();
        }
    }

    public async Task<bool> IsPostLikedAsync(string postId)
    {
        try
        {
            var user = await EnsureUserAsync();
            var response = await _httpClient.GetAsync($"api/blog/{postId}/liked?userId={user.Id}");
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<bool>();
            return result;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<User> EnsureUserAsync()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null || string.IsNullOrWhiteSpace(user.Id))
            throw new InvalidOperationException("You need to be signed in to perform this action.");
        return user;
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url, object? payload = null, bool includeUserHeader = false)
    {
        var request = new HttpRequestMessage(method, url);
        if (payload != null)
        {
            request.Content = JsonContent.Create(payload);
        }

        if (includeUserHeader)
        {
            var user = await EnsureUserAsync();
            request.Headers.Remove("X-User-Id");
            request.Headers.Add("X-User-Id", user.Id);
        }

        return request;
    }
}
