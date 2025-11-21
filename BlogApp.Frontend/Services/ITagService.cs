using BlogApp.Shared.Models;

namespace BlogApp.Frontend.Services;

public interface ITagService
{
    Task<List<Tag>> GetTagsAsync();
    Task<Tag?> GetTagBySlugAsync(string slug);
    Task<List<BlogPost>> GetPostsByTagAsync(string slug, int page = 1, int pageSize = 10);
}

