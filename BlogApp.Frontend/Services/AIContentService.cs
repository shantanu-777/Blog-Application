using BlogApp.Shared.Models.AI;
using System.Net.Http.Json;
using BlogApp.Frontend.Services;

namespace BlogApp.Frontend.Services;

public class AIContentService : IAIContentService
{
    private readonly HttpClient _httpClient;

    public AIContentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ContentGenerationResponse> GenerateContentAsync(ContentGenerationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/generate", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ContentGenerationResponse>() ?? throw new Exception("Failed to generate content");
    }

    public async Task<List<string>> GenerateImageSuggestionsAsync(string content, int count = 5)
    {
        var request = new { content, count };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/generate-images", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
    }

    public async Task<List<string>> GenerateRelatedTopicsAsync(string topic, int count = 5)
    {
        var request = new { topic, count };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/generate-related-topics", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
    }

    public async Task<string> OptimizeContentForSEOAsync(string content, string title)
    {
        var request = new { content, title };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/optimize-seo", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return result?.optimizedContent ?? content;
    }

    public async Task<double> CalculateReadingTimeAsync(string content)
    {
        var request = new { content };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/calculate-reading-time", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return result?.readingTime ?? 0;
    }

    public async Task<List<string>> ExtractKeywordsAsync(string content)
    {
        var request = new { content };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/extract-keywords", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
    }

    public async Task<string> GenerateExcerptAsync(string content, int maxLength = 200)
    {
        var request = new { content, maxLength };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/generate-excerpt", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return result?.excerpt ?? content.Substring(0, Math.Min(maxLength, content.Length));
    }

    public async Task<List<string>> SuggestTagsAsync(string content, int count = 10)
    {
        var request = new { content, count };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/suggest-tags", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
    }

    public async Task<string> ImproveWritingStyleAsync(string content, WritingStyle targetStyle)
    {
        var request = new { content, targetStyle };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/improve-writing-style", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return result?.improvedContent ?? content;
    }

    public async Task<bool> DetectPlagiarismAsync(string content)
    {
        var request = new { content };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/detect-plagiarism", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return result?.isPlagiarized ?? false;
    }

    public async Task<ContentGenerationResponse> GenerateSeriesContentAsync(string seriesTitle, int partNumber, string previousContent = "")
    {
        var request = new { seriesTitle, partNumber, previousContent };
        var response = await _httpClient.PostAsJsonAsync("api/aicontent/generate-series-content", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ContentGenerationResponse>() ?? throw new Exception("Failed to generate series content");
    }
}
