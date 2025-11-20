using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models.AI;

public class ContentGenerationRequest
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Topic { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? AdditionalContext { get; set; }
    
    public ContentType Type { get; set; } = ContentType.BlogPost;
    
    public int TargetLength { get; set; } = 1000;
    
    public WritingStyle Style { get; set; } = WritingStyle.Professional;
    
    public string? TargetAudience { get; set; }
    
    public List<string> Keywords { get; set; } = new();
    
    public bool IncludeImages { get; set; } = true;
    
    public bool IncludeCodeExamples { get; set; } = false;
    
    public string? Language { get; set; } = "en";
}

public enum ContentType
{
    BlogPost = 0,
    Article = 1,
    Tutorial = 2,
    News = 3,
    Review = 4,
    Opinion = 5
}

public enum WritingStyle
{
    Professional = 0,
    Casual = 1,
    Academic = 2,
    Creative = 3,
    Technical = 4,
    Conversational = 5
}

public class ContentGenerationResponse
{
    public string GeneratedContent { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Excerpt { get; set; } = string.Empty;
    
    public List<string> SuggestedTags { get; set; } = new();
    
    public List<string> SuggestedCategories { get; set; } = new();
    
    public int EstimatedReadingTime { get; set; }
    
    public double ConfidenceScore { get; set; }
    
    public List<string> ImageSuggestions { get; set; } = new();
    
    public string? SEOOptimizedTitle { get; set; }
    
    public string? MetaDescription { get; set; }
    
    public List<string> RelatedTopics { get; set; } = new();

    public List<string> Outline { get; set; } = new();
}
