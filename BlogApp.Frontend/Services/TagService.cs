using System.Net.Http.Json;
using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public class TagService : ITagService
{
    private readonly HttpClient _httpClient;

    public TagService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Tag>>("api/tag");
            return response ?? new List<Tag>();
        }
        catch
        {
            return new List<Tag>();
        }
    }

    public async Task<Tag?> GetTagBySlugAsync(string slug)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Tag>($"api/tag/slug/{Uri.EscapeDataString(slug)}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<BlogPost>> GetPostsByTagAsync(string slug, int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BlogPost>>(
                $"api/tag/{Uri.EscapeDataString(slug)}/posts?page={page}&pageSize={pageSize}");
            return response ?? new List<BlogPost>();
        }
        catch
        {
            return new List<BlogPost>();
        }
    }
}

