namespace BlogApp.Shared.Models;

public class AiDraft
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Excerpt { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? FeaturedImageUrl { get; set; }
    public string? MetaDescription { get; set; }
}

