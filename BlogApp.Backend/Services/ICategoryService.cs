using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface ICategoryService
{
    Task<Category> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Category?> GetCategoryAsync(string id);
    Task<Category?> GetCategoryBySlugAsync(string slug);
    Task<List<Category>> GetCategoriesAsync(bool includeInactive = false);
    Task<Category> UpdateCategoryAsync(string id, UpdateCategoryRequest request);
    Task<bool> DeleteCategoryAsync(string id);
    Task<List<BlogPost>> GetPostsByCategoryAsync(string categorySlug, int page = 1, int pageSize = 10);
}

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; } = true;
}

