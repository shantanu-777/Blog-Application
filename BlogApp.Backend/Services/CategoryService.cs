using System.Text.RegularExpressions;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public class CategoryService : ICategoryService
{
    private readonly SupabaseRestClient _client;
    private readonly IBlogService _blogService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        SupabaseRestClient client,
        IBlogService blogService,
        ILogger<CategoryService> logger)
    {
        _client = client;
        _blogService = blogService;
        _logger = logger;
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var slug = GenerateSlug(request.Name);
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Slug = slug,
            Color = request.Color,
            Icon = request.Icon,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var dto = category.ToDto();
        var created = await _client.PostAsync<SupabaseCategoryDto>("categories", dto) ?? dto;
        return created.ToModel();
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        var categories = await _client.GetAsync<SupabaseCategoryDto>($"categories?select=*&id=eq.{Uri.EscapeDataString(id)}&limit=1");
        return categories.FirstOrDefault()?.ToModel();
    }

    public async Task<Category?> GetCategoryBySlugAsync(string slug)
    {
        var categories = await _client.GetAsync<SupabaseCategoryDto>($"categories?select=*&slug=eq.{Uri.EscapeDataString(slug)}&limit=1");
        return categories.FirstOrDefault()?.ToModel();
    }

    public async Task<List<Category>> GetCategoriesAsync(bool includeInactive = false)
    {
        var query = "categories?select=*&order=name.asc";
        if (!includeInactive)
        {
            query += "&is_active=eq.true";
        }
        var categories = await _client.GetAsync<SupabaseCategoryDto>(query);
        return categories.Select(c => c.ToModel()).ToList();
    }

    public async Task<Category> UpdateCategoryAsync(string id, UpdateCategoryRequest request)
    {
        var existing = await GetCategoryAsync(id) ?? throw new ArgumentException("Category not found");
        
        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Color = request.Color;
        existing.Icon = request.Icon;
        existing.IsActive = request.IsActive;
        existing.Slug = GenerateSlug(request.Name);

        var dto = existing.ToDto();
        var updated = await _client.PatchAsync<SupabaseCategoryDto>($"categories?id=eq.{Uri.EscapeDataString(id)}", dto);
        return updated?.ToModel() ?? existing;
    }

    public async Task<bool> DeleteCategoryAsync(string id)
    {
        await _client.DeleteAsync($"categories?id=eq.{Uri.EscapeDataString(id)}");
        return true;
    }

    public async Task<List<BlogPost>> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10)
    {
        var category = await GetCategoryBySlugAsync(categorySlug);
        if (category == null)
            return new List<BlogPost>();

        var posts = await _blogService.GetPostsAsync(new GetPostsRequest
        {
            Category = category.Name,
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

