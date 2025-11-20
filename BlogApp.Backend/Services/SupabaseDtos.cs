using System.Text.Json.Serialization;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

internal static class SupabaseDtoExtensions
{
    public static BlogPost ToModel(this SupabasePostDto dto)
    {
        return new BlogPost
        {
            Id = dto.Id,
            Title = dto.Title,
            Excerpt = dto.Excerpt,
            Content = dto.Content,
            FeaturedImageUrl = dto.FeaturedImageUrl,
            AuthorId = dto.AuthorId,
            Status = dto.Status,
            Visibility = dto.Visibility,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
            PublishedAt = dto.PublishedAt,
            UpdatedAt = dto.UpdatedAt,
            ViewsCount = dto.ViewsCount,
            LikesCount = dto.LikesCount,
            CommentsCount = dto.CommentsCount,
            ReadingTimeMinutes = dto.ReadingTimeMinutes,
            Slug = dto.Slug,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            ScheduledAt = dto.ScheduledAt,
            Tags = dto.Tags?.ToList() ?? new List<string>(),
            Categories = dto.Categories?.ToList() ?? new List<string>(),
            IsAIGenerated = dto.IsAIGenerated,
            AIPrompt = dto.AIPrompt,
            AIConfidence = dto.AIConfidence,
            ReviewNotes = dto.ReviewNotes
        };
    }

    public static SupabasePostDto ToDto(this BlogPost post)
    {
        return new SupabasePostDto
        {
            Id = string.IsNullOrEmpty(post.Id) ? Guid.NewGuid().ToString() : post.Id,
            Title = post.Title,
            Excerpt = post.Excerpt,
            Content = post.Content,
            FeaturedImageUrl = post.FeaturedImageUrl,
            AuthorId = post.AuthorId,
            Status = post.Status,
            Visibility = post.Visibility,
            CreatedAt = post.CreatedAt,
            PublishedAt = post.PublishedAt,
            UpdatedAt = post.UpdatedAt,
            ViewsCount = post.ViewsCount,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            ReadingTimeMinutes = post.ReadingTimeMinutes,
            Slug = post.Slug,
            MetaDescription = post.MetaDescription,
            MetaKeywords = post.MetaKeywords,
            ScheduledAt = post.ScheduledAt,
            Tags = post.Tags?.ToArray() ?? Array.Empty<string>(),
            Categories = post.Categories?.ToArray() ?? Array.Empty<string>(),
            IsAIGenerated = post.IsAIGenerated,
            AIPrompt = post.AIPrompt,
            AIConfidence = post.AIConfidence,
            ReviewNotes = post.ReviewNotes
        };
    }

    public static Comment ToModel(this SupabaseCommentDto dto)
    {
        return new Comment
        {
            Id = dto.Id,
            PostId = dto.PostId,
            AuthorId = dto.AuthorId,
            Content = dto.Content,
            ParentCommentId = dto.ParentCommentId,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = dto.UpdatedAt,
            LikesCount = dto.LikesCount,
            IsDeleted = dto.IsDeleted,
            IsApproved = dto.IsApproved,
            IsFlagged = dto.IsFlagged,
            FlagReason = dto.FlagReason
        };
    }

    public static SupabaseCommentDto ToDto(this Comment comment)
    {
        return new SupabaseCommentDto
        {
            Id = string.IsNullOrEmpty(comment.Id) ? Guid.NewGuid().ToString() : comment.Id,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            LikesCount = comment.LikesCount,
            IsDeleted = comment.IsDeleted,
            IsApproved = comment.IsApproved,
            IsFlagged = comment.IsFlagged,
            FlagReason = comment.FlagReason
        };
    }

    public static SiteSettings ToModel(this SupabaseSiteSettingsDto dto)
    {
        return new SiteSettings
        {
            Id = dto.Id,
            SiteTitle = dto.SiteTitle,
            SiteDescription = dto.SiteDescription,
            SiteUrl = dto.SiteUrl,
            LogoUrl = dto.LogoUrl,
            FaviconUrl = dto.FaviconUrl,
            ThemeColor = dto.ThemeColor,
            PrimaryColor = dto.PrimaryColor,
            SecondaryColor = dto.SecondaryColor,
            AllowComments = dto.AllowComments,
            RequireCommentApproval = dto.RequireCommentApproval,
            AllowUserRegistration = dto.AllowUserRegistration,
            RequireEmailVerification = dto.RequireEmailVerification,
            ContactEmail = dto.ContactEmail,
            SocialMediaLinks = dto.SocialMediaLinks,
            GoogleAnalyticsId = dto.GoogleAnalyticsId,
            FacebookPixelId = dto.FacebookPixelId,
            PostsPerPage = dto.PostsPerPage,
            EnableNewsletter = dto.EnableNewsletter,
            EnableSearch = dto.EnableSearch,
            UpdatedAt = dto.UpdatedAt ?? DateTime.UtcNow
        };
    }

    public static SupabaseSiteSettingsDto ToDto(this SiteSettings settings)
    {
        return new SupabaseSiteSettingsDto
        {
            Id = string.IsNullOrEmpty(settings.Id) ? Guid.NewGuid().ToString() : settings.Id,
            SiteTitle = settings.SiteTitle,
            SiteDescription = settings.SiteDescription,
            SiteUrl = settings.SiteUrl,
            LogoUrl = settings.LogoUrl,
            FaviconUrl = settings.FaviconUrl,
            ThemeColor = settings.ThemeColor,
            PrimaryColor = settings.PrimaryColor,
            SecondaryColor = settings.SecondaryColor,
            AllowComments = settings.AllowComments,
            RequireCommentApproval = settings.RequireCommentApproval,
            AllowUserRegistration = settings.AllowUserRegistration,
            RequireEmailVerification = settings.RequireEmailVerification,
            ContactEmail = settings.ContactEmail,
            SocialMediaLinks = settings.SocialMediaLinks,
            GoogleAnalyticsId = settings.GoogleAnalyticsId,
            FacebookPixelId = settings.FacebookPixelId,
            PostsPerPage = settings.PostsPerPage,
            EnableNewsletter = settings.EnableNewsletter,
            EnableSearch = settings.EnableSearch,
            UpdatedAt = settings.UpdatedAt
        };
    }
}

internal class SupabasePostDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("excerpt")]
    public string Excerpt { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("featured_image_url")]
    public string? FeaturedImageUrl { get; set; }

    [JsonPropertyName("author_id")]
    public string AuthorId { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public PostStatus Status { get; set; } = PostStatus.Draft;

    [JsonPropertyName("visibility")]
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime? PublishedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("views_count")]
    public int ViewsCount { get; set; }

    [JsonPropertyName("likes_count")]
    public int LikesCount { get; set; }

    [JsonPropertyName("comments_count")]
    public int CommentsCount { get; set; }

    [JsonPropertyName("reading_time_minutes")]
    public int ReadingTimeMinutes { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("meta_description")]
    public string? MetaDescription { get; set; }

    [JsonPropertyName("meta_keywords")]
    public string? MetaKeywords { get; set; }

    [JsonPropertyName("scheduled_at")]
    public DateTime? ScheduledAt { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("categories")]
    public string[]? Categories { get; set; }

    [JsonPropertyName("is_ai_generated")]
    public bool IsAIGenerated { get; set; }

    [JsonPropertyName("ai_prompt")]
    public string? AIPrompt { get; set; }

    [JsonPropertyName("ai_confidence")]
    public double AIConfidence { get; set; }

    [JsonPropertyName("review_notes")]
    public string? ReviewNotes { get; set; }
}

internal class SupabaseCommentDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("post_id")]
    public string PostId { get; set; } = string.Empty;

    [JsonPropertyName("author_id")]
    public string AuthorId { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("parent_comment_id")]
    public string? ParentCommentId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("likes_count")]
    public int LikesCount { get; set; }

    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("is_approved")]
    public bool IsApproved { get; set; }

    [JsonPropertyName("is_flagged")]
    public bool IsFlagged { get; set; }

    [JsonPropertyName("flag_reason")]
    public string? FlagReason { get; set; }
}

internal class SupabaseLikeDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("post_id")]
    public string PostId { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
}

internal class SupabaseSiteSettingsDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("site_title")]
    public string SiteTitle { get; set; } = "My Blog";

    [JsonPropertyName("site_description")]
    public string SiteDescription { get; set; } = "A modern blog application";

    [JsonPropertyName("site_url")]
    public string SiteUrl { get; set; } = "https://myblog.com";

    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }

    [JsonPropertyName("favicon_url")]
    public string? FaviconUrl { get; set; }

    [JsonPropertyName("theme_color")]
    public string? ThemeColor { get; set; }

    [JsonPropertyName("primary_color")]
    public string? PrimaryColor { get; set; }

    [JsonPropertyName("secondary_color")]
    public string? SecondaryColor { get; set; }

    [JsonPropertyName("allow_comments")]
    public bool AllowComments { get; set; }

    [JsonPropertyName("require_comment_approval")]
    public bool RequireCommentApproval { get; set; }

    [JsonPropertyName("allow_user_registration")]
    public bool AllowUserRegistration { get; set; }

    [JsonPropertyName("require_email_verification")]
    public bool RequireEmailVerification { get; set; }

    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }

    [JsonPropertyName("social_media_links")]
    public string? SocialMediaLinks { get; set; }

    [JsonPropertyName("google_analytics_id")]
    public string? GoogleAnalyticsId { get; set; }

    [JsonPropertyName("facebook_pixel_id")]
    public string? FacebookPixelId { get; set; }

    [JsonPropertyName("posts_per_page")]
    public int PostsPerPage { get; set; }

    [JsonPropertyName("enable_newsletter")]
    public bool EnableNewsletter { get; set; }

    [JsonPropertyName("enable_search")]
    public bool EnableSearch { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

