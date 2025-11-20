using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;
using BlogApp.Shared.Models.AI;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIContentController : ControllerBase
{
    private readonly IAIContentService _aiContentService;
    private readonly ILogger<AIContentController> _logger;

    public AIContentController(IAIContentService aiContentService, ILogger<AIContentController> logger)
    {
        _aiContentService = aiContentService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ContentGenerationResponse>> GenerateContent(ContentGenerationRequest request)
    {
        try
        {
            var response = await _aiContentService.GenerateContentAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating content");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("generate-images")]
    public async Task<ActionResult<List<string>>> GenerateImageSuggestions([FromBody] GenerateImageRequest request)
    {
        try
        {
            var suggestions = await _aiContentService.GenerateImageSuggestionsAsync(request.Content, request.Count);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image suggestions");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("generate-related-topics")]
    public async Task<ActionResult<List<string>>> GenerateRelatedTopics([FromBody] GenerateRelatedTopicsRequest request)
    {
        try
        {
            var topics = await _aiContentService.GenerateRelatedTopicsAsync(request.Topic, request.Count);
            return Ok(topics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating related topics");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("optimize-seo")]
    public async Task<ActionResult<string>> OptimizeContentForSEO([FromBody] OptimizeSEORequest request)
    {
        try
        {
            var optimizedContent = await _aiContentService.OptimizeContentForSEOAsync(request.Content, request.Title);
            return Ok(new { optimizedContent });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing content for SEO");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("calculate-reading-time")]
    public async Task<ActionResult<double>> CalculateReadingTime([FromBody] CalculateReadingTimeRequest request)
    {
        try
        {
            var readingTime = await _aiContentService.CalculateReadingTimeAsync(request.Content);
            return Ok(new { readingTime });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating reading time");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("extract-keywords")]
    public async Task<ActionResult<List<string>>> ExtractKeywords([FromBody] ExtractKeywordsRequest request)
    {
        try
        {
            var keywords = await _aiContentService.ExtractKeywordsAsync(request.Content);
            return Ok(keywords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting keywords");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("generate-excerpt")]
    public async Task<ActionResult<string>> GenerateExcerpt([FromBody] GenerateExcerptRequest request)
    {
        try
        {
            var excerpt = await _aiContentService.GenerateExcerptAsync(request.Content, request.MaxLength);
            return Ok(new { excerpt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating excerpt");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("suggest-tags")]
    public async Task<ActionResult<List<string>>> SuggestTags([FromBody] SuggestTagsRequest request)
    {
        try
        {
            var tags = await _aiContentService.SuggestTagsAsync(request.Content, request.Count);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suggesting tags");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("improve-writing-style")]
    public async Task<ActionResult<string>> ImproveWritingStyle([FromBody] ImproveWritingStyleRequest request)
    {
        try
        {
            var improvedContent = await _aiContentService.ImproveWritingStyleAsync(request.Content, request.TargetStyle);
            return Ok(new { improvedContent });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error improving writing style");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("detect-plagiarism")]
    public async Task<ActionResult<bool>> DetectPlagiarism([FromBody] DetectPlagiarismRequest request)
    {
        try
        {
            var isPlagiarized = await _aiContentService.DetectPlagiarismAsync(request.Content);
            return Ok(new { isPlagiarized });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting plagiarism");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("generate-series-content")]
    public async Task<ActionResult<ContentGenerationResponse>> GenerateSeriesContent([FromBody] GenerateSeriesContentRequest request)
    {
        try
        {
            var response = await _aiContentService.GenerateSeriesContentAsync(request.SeriesTitle, request.PartNumber, request.PreviousContent);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating series content");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public class GenerateImageRequest
{
    public string Content { get; set; } = string.Empty;
    public int Count { get; set; } = 5;
}

public class GenerateRelatedTopicsRequest
{
    public string Topic { get; set; } = string.Empty;
    public int Count { get; set; } = 5;
}

public class OptimizeSEORequest
{
    public string Content { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class CalculateReadingTimeRequest
{
    public string Content { get; set; } = string.Empty;
}

public class ExtractKeywordsRequest
{
    public string Content { get; set; } = string.Empty;
}

public class GenerateExcerptRequest
{
    public string Content { get; set; } = string.Empty;
    public int MaxLength { get; set; } = 200;
}

public class SuggestTagsRequest
{
    public string Content { get; set; } = string.Empty;
    public int Count { get; set; } = 10;
}

public class ImproveWritingStyleRequest
{
    public string Content { get; set; } = string.Empty;
    public WritingStyle TargetStyle { get; set; }
}

public class DetectPlagiarismRequest
{
    public string Content { get; set; } = string.Empty;
}

public class GenerateSeriesContentRequest
{
    public string SeriesTitle { get; set; } = string.Empty;
    public int PartNumber { get; set; }
    public string PreviousContent { get; set; } = string.Empty;
}
