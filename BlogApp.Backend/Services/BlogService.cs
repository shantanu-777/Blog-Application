using System.Text.RegularExpressions;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public class BlogService : IBlogService
{
    private readonly ISupabaseBlogRepository _repository;
    private readonly ILogger<BlogService> _logger;
    private readonly ISiteSettingsService _siteSettingsService;

    public BlogService(
        ISupabaseBlogRepository repository,
        ILogger<BlogService> logger,
        ISiteSettingsService siteSettingsService)
    {
        _repository = repository;
        _logger = logger;
        _siteSettingsService = siteSettingsService;
    }

    public async Task<BlogPost> CreatePostAsync(CreatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AuthorId))
            throw new ArgumentException("AuthorId is required to create a post.");

        var now = DateTime.UtcNow;
        var slug = await GenerateUniqueSlugAsync(request.Title);

        var post = new BlogPost
        {
            Title = request.Title,
            Excerpt = string.IsNullOrWhiteSpace(request.Excerpt) ? BuildExcerpt(request.Content) : request.Excerpt,
            Content = request.Content,
            FeaturedImageUrl = request.FeaturedImageUrl,
            AuthorId = request.AuthorId,
            Status = PostStatus.Published,
            Visibility = request.Visibility,
            CreatedAt = now,
            PublishedAt = now,
            UpdatedAt = now,
            Tags = request.Tags ?? new List<string>(),
            Categories = request.Categories ?? new List<string>(),
            MetaDescription = request.MetaDescription,
            MetaKeywords = request.MetaKeywords,
            ScheduledAt = request.ScheduledAt,
            IsAIGenerated = request.IsAIGenerated,
            AIPrompt = request.AIPrompt,
            AIConfidence = request.AIConfidence == 0 ? 0.85 : request.AIConfidence,
            ReadingTimeMinutes = CalculateReadingTime(request.Content),
            Slug = slug
        };

        var created = await _repository.InsertPostAsync(post);
        return created;
    }

    public Task<BlogPost?> GetPostAsync(string id) => _repository.GetPostByIdAsync(id);

    public Task<BlogPost?> GetPostBySlugAsync(string slug) => _repository.GetPostBySlugAsync(slug);

    public Task<List<BlogPost>> GetPostsAsync(GetPostsRequest request) => _repository.GetPostsAsync(request);

    public async Task<BlogPost> UpdatePostAsync(string id, UpdatePostRequest request)
    {
        var existing = await _repository.GetPostByIdAsync(id) ?? throw new ArgumentException("Post not found");

        existing.Title = request.Title;
        existing.Excerpt = request.Excerpt;
        existing.Content = request.Content;
        existing.FeaturedImageUrl = request.FeaturedImageUrl;
        existing.Status = request.Status;
        existing.Visibility = request.Visibility;
        existing.Tags = request.Tags ?? new List<string>();
        existing.Categories = request.Categories ?? new List<string>();
        existing.MetaDescription = request.MetaDescription;
        existing.MetaKeywords = request.MetaKeywords;
        existing.ScheduledAt = request.ScheduledAt;
        existing.ReadingTimeMinutes = CalculateReadingTime(request.Content);
        existing.UpdatedAt = DateTime.UtcNow;
        existing.Slug = await EnsureSlugForUpdateAsync(existing.Slug, existing.Title, id);

        var updated = await _repository.UpdatePostAsync(id, existing) ?? existing;
        return updated;
    }

    public Task<bool> DeletePostAsync(string id) => _repository.DeletePostAsync(id);

    public async Task<bool> LikePostAsync(string postId, string userId)
    {
        if (await _repository.HasUserLikedAsync(postId, userId))
            return true;

        await _repository.InsertLikeAsync(postId, userId);
        await SyncLikesCountAsync(postId);
        return true;
    }

    public async Task<bool> UnlikePostAsync(string postId, string userId)
    {
        await _repository.DeleteLikeAsync(postId, userId);
        await SyncLikesCountAsync(postId);
        return true;
    }

    public Task<List<Comment>> GetCommentsAsync(string postId) => _repository.GetCommentsAsync(postId);

    public async Task<Comment> AddCommentAsync(AddCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AuthorId))
            throw new ArgumentException("AuthorId is required for comments.");

        var settings = await _siteSettingsService.GetSettingsAsync();
        if (!settings.AllowComments)
            throw new InvalidOperationException("Comments are disabled for this site.");

        var comment = new Comment
        {
            PostId = request.PostId,
            AuthorId = request.AuthorId,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            CreatedAt = DateTime.UtcNow,
            IsApproved = !settings.RequireCommentApproval,
            IsFlagged = settings.RequireCommentApproval
        };

        var created = await _repository.InsertCommentAsync(comment);
        if (created.IsApproved)
        {
            await UpdateCommentsCountAsync(request.PostId, 1);
        }
        return created;
    }

    public async Task<Comment> UpdateCommentAsync(string id, UpdateCommentRequest request)
    {
        var updated = await _repository.UpdateCommentAsync(id, request.Content)
                      ?? throw new ArgumentException("Comment not found");
        return updated;
    }

    public async Task<bool> DeleteCommentAsync(string id)
    {
        var comment = await _repository.GetCommentByIdAsync(id);
        if (comment == null)
            return false;

        await _repository.DeleteCommentAsync(id);
        if (comment.IsApproved && !comment.IsDeleted)
        {
            await UpdateCommentsCountAsync(comment.PostId, -1);
        }
        return true;
    }

    public async Task<List<BlogPost>> GetTrendingPostsAsync(int count = 10)
    {
        var posts = await _repository.GetPostsAsync(new GetPostsRequest
        {
            PageSize = count,
            SortBy = "likes_count",
            SortOrder = "desc",
            Status = PostStatus.Published
        });

        return posts;
    }

    public async Task<List<BlogPost>> GetRecommendedPostsAsync(string userId, int count = 10)
    {
        var userPosts = await _repository.GetPostsAsync(new GetPostsRequest
        {
            AuthorId = userId,
            PageSize = 100
        });

        var favoriteTags = userPosts
            .SelectMany(p => p.Tags ?? new List<string>())
            .GroupBy(t => t.ToLowerInvariant())
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key)
            .ToHashSet();

        if (!favoriteTags.Any())
        {
            return await GetTrendingPostsAsync(count);
        }

        var allPosts = await _repository.GetPostsAsync(new GetPostsRequest
        {
            PageSize = 200,
            Status = PostStatus.Published
        });

        return allPosts
            .Where(p => p.AuthorId != userId && (p.Tags?.Any(tag => favoriteTags.Contains(tag.ToLowerInvariant())) ?? false))
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToList();
    }

    public async Task<List<BlogPost>> SearchPostsAsync(string query, int page = 1, int pageSize = 10)
    {
        var posts = await _repository.GetPostsAsync(new GetPostsRequest
        {
            SearchQuery = query,
            Page = page,
            PageSize = pageSize,
            Status = PostStatus.Published
        });

        return posts;
    }

    public async Task<List<BlogPost>> GetRelatedPostsAsync(string postId, int count = 3)
    {
        var post = await _repository.GetPostByIdAsync(postId);
        if (post == null)
            return new List<BlogPost>();

        var posts = await _repository.GetPostsAsync(new GetPostsRequest
        {
            PageSize = 100,
            Status = PostStatus.Published
        });

        return posts
            .Where(p => p.Id != postId &&
                        (SharesAny(p.Tags, post.Tags) ||
                         SharesAny(p.Categories, post.Categories) ||
                         p.AuthorId == post.AuthorId))
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToList();
    }

    public Task<bool> IsPostLikedAsync(string postId, string userId)
        => _repository.HasUserLikedAsync(postId, userId);

    private async Task SyncLikesCountAsync(string postId)
    {
        var post = await _repository.GetPostByIdAsync(postId);
        if (post == null) return;

        post.LikesCount = await _repository.CountLikesAsync(postId);
        post.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdatePostAsync(postId, post);
    }

    private async Task UpdateCommentsCountAsync(string postId, int delta)
    {
        var post = await _repository.GetPostByIdAsync(postId);
        if (post == null) return;

        post.CommentsCount = Math.Max(0, post.CommentsCount + delta);
        post.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdatePostAsync(postId, post);
    }

    private async Task<string> GenerateUniqueSlugAsync(string title)
    {
        var baseSlug = GenerateSlug(title);
        var slug = baseSlug;
        var suffix = 1;

        while (await _repository.GetPostBySlugAsync(slug) != null)
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }

    private async Task<string> EnsureSlugForUpdateAsync(string? currentSlug, string title, string postId)
    {
        if (string.IsNullOrWhiteSpace(currentSlug))
            return await GenerateUniqueSlugAsync(title);

        var desiredSlug = GenerateSlug(title);
        if (desiredSlug == currentSlug)
            return currentSlug;

        var existing = await _repository.GetPostBySlugAsync(desiredSlug);
        if (existing == null || existing.Id == postId)
            return desiredSlug;

        return await GenerateUniqueSlugAsync(title);
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, "-+", "-");
        return slug.Trim('-');
    }

    private static int CalculateReadingTime(string content)
    {
        var wordsPerMinute = 200;
        var wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(wordCount / (double)wordsPerMinute));
    }

    private static string BuildExcerpt(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var trimmed = content.Trim();
        return trimmed.Length > 300 ? $"{trimmed.Substring(0, 300)}..." : trimmed;
    }

    private static bool SharesAny(IEnumerable<string>? first, IEnumerable<string>? second)
    {
        if (first == null || second == null)
            return false;

        var hash = new HashSet<string>(second.Select(s => s.ToLowerInvariant()));
        return first.Any(item => hash.Contains(item.ToLowerInvariant()));
    }
}
