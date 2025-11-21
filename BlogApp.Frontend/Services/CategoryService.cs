using System.Net.Http.Json;
using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Category>>("api/category");
            return response ?? new List<Category>();
        }
        catch
        {
            return new List<Category>();
        }
    }

    public async Task<Category?> GetCategoryBySlugAsync(string slug)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Category>($"api/category/slug/{Uri.EscapeDataString(slug)}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<BlogPost>> GetPostsByCategoryAsync(string slug, int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>(
                $"api/category/{Uri.EscapeDataString(slug)}/posts?page={page}&pageSize={pageSize}");
            return response ?? new List<BlogPost>();
        }
        catch
        {
            return new List<BlogPost>();
        }
    }
}

