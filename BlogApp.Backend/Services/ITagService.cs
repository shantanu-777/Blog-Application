using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface ITagService
{
    Task<Tag> CreateTagAsync(CreateTagRequest request);
    Task<Tag?> GetTagAsync(string id);
    Task<Tag?> GetTagBySlugAsync(string slug);
    Task<List<Tag>> GetTagsAsync(bool includeInactive = false);
    Task<Tag> UpdateTagAsync(string id, UpdateTagRequest request);
    Task<bool> DeleteTagAsync(string id);
    Task<List<BlogPost>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10);
}

public class CreateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class UpdateTagRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
}

