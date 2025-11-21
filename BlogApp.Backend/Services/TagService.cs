using System.Text.RegularExpressions;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public class TagService : ITagService
{
    private readonly SupabaseRestClient _client;
    private readonly IBlogService _blogService;
    private readonly ILogger<TagService> _logger;

    public TagService(
        SupabaseRestClient client,
        IBlogService blogService,
        ILogger<TagService> logger)
    {
        _client = client;
        _blogService = blogService;
        _logger = logger;
    }

    public async Task<Tag> CreateTagAsync(CreateTagRequest request)
    {
        var slug = GenerateSlug(request.Name);
        var tag = new Tag
        {
            Name = request.Name,
            Description = request.Description,
            Slug = slug,
            Color = request.Color,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var dto = tag.ToDto();
        var created = await _client.PostAsync<SupabaseTagDto>("tags", dto) ?? dto;
        return created.ToModel();
    }

    public async Task<Tag?> GetTagAsync(string id)
    {
        var tags = await _client.GetAsync<SupabaseTagDto>($"tags?select=*&id=eq.{Uri.EscapeDataString(id)}&limit=1");
        return tags.FirstOrDefault()?.ToModel();
    }

    public async Task<Tag?> GetTagBySlugAsync(string slug)
    {
        var tags = await _client.GetAsync<SupabaseTagDto>($"tags?select=*&slug=eq.{Uri.EscapeDataString(slug)}&limit=1");
        return tags.FirstOrDefault()?.ToModel();
    }

    public async Task<List<Tag>> GetTagsAsync(bool includeInactive = false)
    {
        var query = "tags?select=*&order=name.asc";
        if (!includeInactive)
        {
            query += "&is_active=eq.true";
        }
        var tags = await _client.GetAsync<SupabaseTagDto>(query);
        return tags.Select(t => t.ToModel()).ToList();
    }

    public async Task<Tag> UpdateTagAsync(string id, UpdateTagRequest request)
    {
        var existing = await GetTagAsync(id) ?? throw new ArgumentException("Tag not found");
        
        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Color = request.Color;
        existing.IsActive = request.IsActive;
        existing.Slug = GenerateSlug(request.Name);

        var dto = existing.ToDto();
        var updated = await _client.PatchAsync<SupabaseTagDto>($"tags?id=eq.{Uri.EscapeDataString(id)}", dto);
        return updated?.ToModel() ?? existing;
    }

    public async Task<bool> DeleteTagAsync(string id)
    {
        await _client.DeleteAsync($"tags?id=eq.{Uri.EscapeDataString(id)}");
        return true;
    }

    public async Task<List<BlogPost>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10)
    {
        var tag = await GetTagBySlugAsync(tagSlug);
        if (tag == null)
            return new List<BlogPost>();

        var posts = await _blogService.GetPostsAsync(new GetPostsRequest
        {
            Tag = tag.Name,
            Page = page,
            PageSize = pageSize,
            Status = PostStatus.Published
        });

        return posts;
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, "-+", "-");
        return slug.Trim('-');
    }
}

