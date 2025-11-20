using BlogApp.Shared.Models.AI;

namespace BlogApp.Frontend.Services;

public interface IAIContentService
{
    Task<ContentGenerationResponse> GenerateContentAsync(ContentGenerationRequest request);
    Task<List<string>> GenerateImageSuggestionsAsync(string content, int count = 5);
    Task<List<string>> GenerateRelatedTopicsAsync(string topic, int count = 5);
    Task<string> OptimizeContentForSEOAsync(string content, string title);
    Task<double> CalculateReadingTimeAsync(string content);
    Task<List<string>> ExtractKeywordsAsync(string content);
    Task<string> GenerateExcerptAsync(string content, int maxLength = 200);
    Task<List<string>> SuggestTagsAsync(string content, int count = 10);
    Task<string> ImproveWritingStyleAsync(string content, WritingStyle targetStyle);
    Task<bool> DetectPlagiarismAsync(string content);
    Task<ContentGenerationResponse> GenerateSeriesContentAsync(string seriesTitle, int partNumber, string previousContent = "");
}
