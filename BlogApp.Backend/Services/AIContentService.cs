using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlogApp.Shared.Models.AI;
using System.Text.RegularExpressions;

namespace BlogApp.Backend.Services
{
    public class AIContentService : IAIContentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIContentService> _logger;
        private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public AIContentService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AIContentService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ContentGenerationResponse> GenerateContentAsync(ContentGenerationRequest request)
        {
            try
            {
                var apiKey = _configuration["AI:GeminiApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured.");
                var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

                var promptBuilder = new StringBuilder();
                promptBuilder.AppendLine("You are an AI writing assistant that produces detailed long-form articles.");
                promptBuilder.AppendLine("Respond strictly in JSON format matching this schema:");
                promptBuilder.AppendLine("{");
                promptBuilder.AppendLine("  \"title\": \"string\",");
                promptBuilder.AppendLine("  \"excerpt\": \"string (<=200 chars)\",");
                promptBuilder.AppendLine("  \"content\": \"multi-paragraph HTML\",");
                promptBuilder.AppendLine("  \"seo_title\": \"string\",");
                promptBuilder.AppendLine("  \"meta_description\": \"string\",");
                promptBuilder.AppendLine("  \"tags\": [\"string\"],");
                promptBuilder.AppendLine("  \"categories\": [\"string\"],");
                promptBuilder.AppendLine("  \"image_suggestions\": [\"string url or prompt\"],");
                promptBuilder.AppendLine("  \"related_topics\": [\"string\"],");
                promptBuilder.AppendLine("  \"outline\": [\"heading and summary\"],");
                promptBuilder.AppendLine("  \"reading_time_minutes\": number,");
                promptBuilder.AppendLine("  \"confidence\": number");
                promptBuilder.AppendLine("}");
                promptBuilder.AppendLine();
                promptBuilder.AppendLine($"Topic: {request.Topic}");
                promptBuilder.AppendLine($"Content type: {request.Type}");
                promptBuilder.AppendLine($"Writing style: {request.Style}");
                promptBuilder.AppendLine($"Target audience: {request.TargetAudience ?? "general readers"}");
                promptBuilder.AppendLine($"Target length: {request.TargetLength} words");
                promptBuilder.AppendLine($"Language: {request.Language ?? "en"}");

                if (!string.IsNullOrWhiteSpace(request.AdditionalContext))
                {
                    promptBuilder.AppendLine("Additional context:");
                    promptBuilder.AppendLine(request.AdditionalContext);
                }

                if (request.Keywords?.Any() == true)
                {
                    promptBuilder.AppendLine($"Must include keywords: {string.Join(", ", request.Keywords)}");
                }

                if (request.IncludeCodeExamples)
                {
                    promptBuilder.AppendLine("Include code examples when relevant.");
                }

                var geminiRequest = new
                {
                    contents = new[]
                    {
                        new
                        {
                            role = "user",
                            parts = new[]
                            {
                                new { text = promptBuilder.ToString() }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.65,
                        topP = 0.9,
                        maxOutputTokens = 2048,
                        response_mime_type = "application/json"
                    }
                };

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync(endpoint, geminiRequest);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini request failed {Status}: {Body}", response.StatusCode, errorBody);
                return BuildFailureResponse(request, $"Gemini API returned {(int)response.StatusCode}. Check API key and model access.");
            }

            var raw = await response.Content.ReadAsStringAsync();
            var jsonPayload = ExtractCandidateJson(raw);

            GeminiStructuredResponse? structured = null;
            if (!string.IsNullOrWhiteSpace(jsonPayload))
            {
                structured = JsonSerializer.Deserialize<GeminiStructuredResponse>(jsonPayload, _serializerOptions);
            }

            var generated = structured?.Content ?? structured?.Body ?? jsonPayload ?? string.Empty;
            var excerpt = structured?.Excerpt ?? BuildExcerpt(generated);

            return new ContentGenerationResponse
            {
                Title = structured?.Title ?? $"AI Generated: {request.Topic}",
                Excerpt = excerpt,
                GeneratedContent = generated,
                SuggestedTags = structured?.Tags ?? BuildFallbackTags(request),
                SuggestedCategories = structured?.Categories ?? new List<string> { request.Type.ToString() },
                ImageSuggestions = structured?.ImageSuggestions ?? new List<string>(),
                SEOOptimizedTitle = structured?.SeoTitle ?? structured?.Title,
                MetaDescription = structured?.MetaDescription ?? excerpt,
                RelatedTopics = structured?.RelatedTopics ?? new List<string>(),
                Outline = structured?.Outline ?? new List<string>(),
                EstimatedReadingTime = structured?.ReadingTimeMinutes ?? CalculateReadingTime(generated),
                ConfidenceScore = structured?.Confidence ?? 0.85
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI content.");
            return BuildFailureResponse(request, ex.Message);
        }
    }

        public async Task<List<string>> GenerateImageSuggestionsAsync(string content, int count = 5)
        {
            var suggestions = new List<string>
            {
                "https://images.unsplash.com/photo-1486312338219-ce68d2c6f44d?w=800",
                "https://images.unsplash.com/photo-1516321318423-f06f85b504d3?w=800",
                "https://images.unsplash.com/photo-1551650975-87deedd944c3?w=800",
                "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800",
                "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=800"
            };

            return suggestions.Take(count).ToList();
        }

        public async Task<List<string>> GenerateRelatedTopicsAsync(string topic, int count = 5)
        {
            var relatedTopics = new List<string>
            {
                $"{topic} best practices",
                $"{topic} tutorial",
                $"{topic} tips and tricks",
                $"{topic} common mistakes",
                $"{topic} advanced techniques"
            };

            return relatedTopics.Take(count).ToList();
        }

        public async Task<string> OptimizeContentForSEOAsync(string content, string title)
        {
            var optimizedContent = content;

            var titleWords = title.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in titleWords)
            {
                if (word.Length > 3 && !optimizedContent.ToLower().Contains(word))
                {
                    optimizedContent = $"{word} {optimizedContent}";
                }
            }

            return optimizedContent;
        }

        public async Task<double> CalculateReadingTimeAsync(string content)
        {
            var wordsPerMinute = 200;
            var wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Ceiling((double)wordCount / wordsPerMinute);
        }

        public async Task<List<string>> ExtractKeywordsAsync(string content)
        {
            var words = content.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 3)
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => g.Key)
                .ToList();

            return words;
        }

        public async Task<string> GenerateExcerptAsync(string content, int maxLength = 200)
        {
            if (content.Length <= maxLength)
                return content;

            var excerpt = content.Substring(0, maxLength);
            var lastSpace = excerpt.LastIndexOf(' ');
            if (lastSpace > 0)
                excerpt = excerpt.Substring(0, lastSpace);

            return excerpt + "...";
        }

        public async Task<List<string>> SuggestTagsAsync(string content, int count = 10)
        {
            var keywords = await ExtractKeywordsAsync(content);
            var commonTags = new List<string> { "technology", "programming", "tutorial", "guide", "tips", "development", "coding", "software", "web", "mobile" };

            return keywords.Concat(commonTags).Distinct().Take(count).ToList();
        }

        public async Task<string> ImproveWritingStyleAsync(string content, WritingStyle targetStyle)
        {
            // For now: mock implementation
            return content;
        }

        public async Task<bool> DetectPlagiarismAsync(string content)
        {
            // For now: mock implementation
            return false;
        }

        public async Task<ContentGenerationResponse> GenerateSeriesContentAsync(string seriesTitle, int partNumber, string previousContent = "")
        {
            var request = new ContentGenerationRequest
            {
                Topic = $"{seriesTitle} - Part {partNumber}",
                AdditionalContext = previousContent,
                Type = ContentType.Tutorial
            };

            return await GenerateContentAsync(request);
        }

        private static string BuildExcerpt(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var clean = Regex.Replace(content, "<.*?>", string.Empty);
            clean = clean.Trim();
            return clean.Length > 220 ? $"{clean[..220]}..." : clean;
        }

        private static int CalculateReadingTime(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 1;

            var wordCount = Regex.Split(content, @"\s+").Length;
            return Math.Max(1, (int)Math.Ceiling(wordCount / 200d));
        }

        private static ContentGenerationResponse BuildFailureResponse(ContentGenerationRequest request, string? reason = null)
        {
            return new ContentGenerationResponse
            {
                GeneratedContent = string.Empty,
                Title = "Generation failed",
                Excerpt = "The AI could not create content at this time.",
                SuggestedTags = BuildFallbackTags(request),
                SuggestedCategories = new List<string> { request.Type.ToString() },
                EstimatedReadingTime = 1,
                ConfidenceScore = 0,
                MetaDescription = reason
            };
        }

        private static List<string> BuildFallbackTags(ContentGenerationRequest request)
        {
            var tags = new List<string>
            {
                request.Topic.ToLowerInvariant(),
                request.Type.ToString().ToLowerInvariant()
            };

            if (request.Keywords?.Any() == true)
                tags.AddRange(request.Keywords.Take(5));

            return tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string? ExtractCandidateJson(string raw)
        {
            try
            {
                using var document = JsonDocument.Parse(raw);
                var candidates = document.RootElement.GetProperty("candidates");
                if (candidates.ValueKind != JsonValueKind.Array || candidates.GetArrayLength() == 0)
                    return raw;

                return candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
            }
            catch
            {
                return raw;
            }
        }

        private class GeminiStructuredResponse
        {
            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("excerpt")]
            public string? Excerpt { get; set; }

            [JsonPropertyName("content")]
            public string? Content { get; set; }

            [JsonPropertyName("body")]
            public string? Body { get; set; }

            [JsonPropertyName("seo_title")]
            public string? SeoTitle { get; set; }

            [JsonPropertyName("meta_description")]
            public string? MetaDescription { get; set; }

            [JsonPropertyName("tags")]
            public List<string>? Tags { get; set; }

            [JsonPropertyName("categories")]
            public List<string>? Categories { get; set; }

            [JsonPropertyName("image_suggestions")]
            public List<string>? ImageSuggestions { get; set; }

            [JsonPropertyName("related_topics")]
            public List<string>? RelatedTopics { get; set; }

            [JsonPropertyName("outline")]
            public List<string>? Outline { get; set; }

            [JsonPropertyName("reading_time_minutes")]
            public int? ReadingTimeMinutes { get; set; }

            [JsonPropertyName("confidence")]
            public double? Confidence { get; set; }
        }
    }
}
