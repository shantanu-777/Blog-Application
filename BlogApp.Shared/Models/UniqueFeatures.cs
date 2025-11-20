using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models;

// Unique features that don't exist in other blog applications
public class SmartReadingList
{
    public string Id { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public List<string> PostIds { get; set; } = new();
    
    public bool IsAICurated { get; set; } = false;
    
    public string? AICriteria { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastUpdatedAt { get; set; }
    
    public int SubscribersCount { get; set; } = 0;
    
    public bool IsPublic { get; set; } = false;
}

public class WritingChallenge
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;
    
    public string? Prompt { get; set; }
    
    public ChallengeType Type { get; set; } = ChallengeType.Daily;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    public int MaxParticipants { get; set; } = 0; // 0 = unlimited
    
    public List<string> ParticipantIds { get; set; } = new();
    
    public List<string> SubmissionIds { get; set; } = new();
    
    public string? WinnerId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum ChallengeType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Special = 3
}

public class CollaborationSpace
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string OwnerId { get; set; } = string.Empty;
    
    public List<string> MemberIds { get; set; } = new();
    
    public List<string> PostIds { get; set; } = new();
    
    public CollaborationType Type { get; set; } = CollaborationType.Private;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastActivityAt { get; set; }
    
    public int MaxMembers { get; set; } = 10;
}

public enum CollaborationType
{
    Private = 0,
    Public = 1,
    InviteOnly = 2
}

public class ReadingInsight
{
    public string Id { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    
    public string PostId { get; set; } = string.Empty;
    
    public int ReadingProgress { get; set; } = 0; // Percentage
    
    public int TimeSpentSeconds { get; set; } = 0;
    
    public List<string> HighlightedTexts { get; set; } = new();
    
    public List<string> BookmarkedSections { get; set; } = new();
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    public double EngagementScore { get; set; } = 0.0;
}

public class ContentSeries
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string AuthorId { get; set; } = string.Empty;
    
    public List<string> PostIds { get; set; } = new();
    
    public int Order { get; set; } = 0;
    
    public bool IsPublished { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? PublishedAt { get; set; }
    
    public int SubscribersCount { get; set; } = 0;
    
    public string? CoverImageUrl { get; set; }
}
