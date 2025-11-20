using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models;

public class BlogPost
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Excerpt { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public string? FeaturedImageUrl { get; set; }
    
    public string AuthorId { get; set; } = string.Empty;
    
    public User? Author { get; set; }
    
    public PostStatus Status { get; set; } = PostStatus.Draft;
    
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? PublishedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public int ViewsCount { get; set; } = 0;
    
    public int LikesCount { get; set; } = 0;
    
    public int CommentsCount { get; set; } = 0;
    
    public int ReadingTimeMinutes { get; set; } = 0;
    
    public string? Slug { get; set; }
    
    public string? MetaDescription { get; set; }
    
    public string? MetaKeywords { get; set; }
    
    public DateTime? ScheduledAt { get; set; }

    public string? ReviewNotes { get; set; }
    
    public List<string> Tags { get; set; } = new();
    
    public List<string> Categories { get; set; } = new();
    
    public bool IsAIGenerated { get; set; } = false;
    
    public string? AIPrompt { get; set; }
    
    public double AIConfidence { get; set; } = 0.0;
    
    public List<Comment> Comments { get; set; } = new();
    
    public List<Like> Likes { get; set; } = new();
}

public enum PostStatus
{
    Draft = 0,
    PendingReview = 1,
    Published = 2,
    Archived = 3
}

public enum PostVisibility
{
    Public = 0,
    Private = 1,
    Unlisted = 2
}

public class Comment
{
    public string Id { get; set; } = string.Empty;
    
    public string PostId { get; set; } = string.Empty;
    
    public string AuthorId { get; set; } = string.Empty;
    
    public User? Author { get; set; }
    
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
    
    public string? ParentCommentId { get; set; }
    
    public List<Comment> Replies { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public int LikesCount { get; set; } = 0;
    
    public bool IsDeleted { get; set; } = false;

    public bool IsApproved { get; set; } = true;

    public bool IsFlagged { get; set; } = false;

    public string? FlagReason { get; set; }
}

public class Like
{
    public string Id { get; set; } = string.Empty;
    
    public string PostId { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    public User? User { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreatePostRequest
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Excerpt { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public string? FeaturedImageUrl { get; set; }
    
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    
    public List<string> Tags { get; set; } = new();
    
    public List<string> Categories { get; set; } = new();
    
    public string? MetaDescription { get; set; }
    
    public string? MetaKeywords { get; set; }
    
    public DateTime? ScheduledAt { get; set; }
    
    public bool IsAIGenerated { get; set; } = false;
    
    public string? AIPrompt { get; set; }
    
    public double AIConfidence { get; set; } = 0.0;

    [Required]
    public string AuthorId { get; set; } = string.Empty;
}

public class UpdatePostRequest
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Excerpt { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public string? FeaturedImageUrl { get; set; }
    
    public PostStatus Status { get; set; } = PostStatus.Draft;
    
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    
    public List<string> Tags { get; set; } = new();
    
    public List<string> Categories { get; set; } = new();
    
    public string? MetaDescription { get; set; }
    
    public string? MetaKeywords { get; set; }
    
    public DateTime? ScheduledAt { get; set; }
}

public class UpdateProfileRequest
{
    [StringLength(100, MinimumLength = 2)]
    public string? DisplayName { get; set; }
    
    [StringLength(500, MinimumLength = 10)]
    public string? Bio { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public string? WebsiteUrl { get; set; }
    
    public string? Location { get; set; }
    
    public List<string> Interests { get; set; } = new();
}
