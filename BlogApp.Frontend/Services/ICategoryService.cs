using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryBySlugAsync(string slug);
    Task<List<BlogPost>> GetPostsByCategoryAsync(string slug, int page = 1, int pageSize = 10);
}

