using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface IUniqueFeaturesService
{
    // Smart Reading Lists
    Task<SmartReadingList> CreateReadingListAsync(CreateReadingListRequest request);
    Task<SmartReadingList?> GetReadingListAsync(string id);
    Task<List<SmartReadingList>> GetUserReadingListsAsync(string userId);
    Task<SmartReadingList> UpdateReadingListAsync(string id, UpdateReadingListRequest request);
    Task<bool> DeleteReadingListAsync(string id);
    Task<bool> AddPostToReadingListAsync(string listId, string postId);
    Task<bool> RemovePostFromReadingListAsync(string listId, string postId);
    Task<List<SmartReadingList>> GetPublicReadingListsAsync(int page = 1, int pageSize = 10);
    Task<bool> SubscribeToReadingListAsync(string listId, string userId);
    Task<bool> UnsubscribeFromReadingListAsync(string listId, string userId);

    // Writing Challenges
    Task<WritingChallenge> CreateChallengeAsync(CreateChallengeRequest request);
    Task<WritingChallenge?> GetChallengeAsync(string id);
    Task<List<WritingChallenge>> GetActiveChallengesAsync();
    Task<List<WritingChallenge>> GetUserChallengesAsync(string userId);
    Task<WritingChallenge> UpdateChallengeAsync(string id, UpdateChallengeRequest request);
    Task<bool> DeleteChallengeAsync(string id);
    Task<bool> JoinChallengeAsync(string challengeId, string userId);
    Task<bool> LeaveChallengeAsync(string challengeId, string userId);
    Task<bool> SubmitToChallengeAsync(string challengeId, string userId, string postId);
    Task<List<WritingChallenge>> GetChallengesByTypeAsync(ChallengeType type);

    // Collaboration Spaces
    Task<CollaborationSpace> CreateSpaceAsync(CreateSpaceRequest request);
    Task<CollaborationSpace?> GetSpaceAsync(string id);
    Task<List<CollaborationSpace>> GetUserSpacesAsync(string userId);
    Task<CollaborationSpace> UpdateSpaceAsync(string id, UpdateSpaceRequest request);
    Task<bool> DeleteSpaceAsync(string id);
    Task<bool> JoinSpaceAsync(string spaceId, string userId);
    Task<bool> LeaveSpaceAsync(string spaceId, string userId);
    Task<bool> InviteToSpaceAsync(string spaceId, string userId, string inviteeEmail);
    Task<List<CollaborationSpace>> GetPublicSpacesAsync(int page = 1, int pageSize = 10);

    // Reading Insights
    Task<ReadingInsight> CreateReadingInsightAsync(CreateReadingInsightRequest request);
    Task<List<ReadingInsight>> GetUserReadingInsightsAsync(string userId);
    Task<ReadingInsight> UpdateReadingProgressAsync(string postId, string userId, int progress);
    Task<List<ReadingInsight>> GetPostReadingInsightsAsync(string postId);
    Task<double> GetUserEngagementScoreAsync(string userId);

    // Content Series
    Task<ContentSeries> CreateSeriesAsync(CreateSeriesRequest request);
    Task<ContentSeries?> GetSeriesAsync(string id);
    Task<List<ContentSeries>> GetUserSeriesAsync(string userId);
    Task<ContentSeries> UpdateSeriesAsync(string id, UpdateSeriesRequest request);
    Task<bool> DeleteSeriesAsync(string id);
    Task<bool> AddPostToSeriesAsync(string seriesId, string postId, int order);
    Task<bool> RemovePostFromSeriesAsync(string seriesId, string postId);
    Task<List<ContentSeries>> GetPublishedSeriesAsync(int page = 1, int pageSize = 10);
    Task<bool> SubscribeToSeriesAsync(string seriesId, string userId);
    Task<bool> UnsubscribeFromSeriesAsync(string seriesId, string userId);
}

public class CreateReadingListRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public bool IsAICurated { get; set; } = false;
    public string? AICriteria { get; set; }
}

public class UpdateReadingListRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
    public bool? IsAICurated { get; set; }
    public string? AICriteria { get; set; }
}

public class CreateChallengeRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Prompt { get; set; }
    public ChallengeType Type { get; set; } = ChallengeType.Daily;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; } = 0;
}

public class UpdateChallengeRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Prompt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxParticipants { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateSpaceRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CollaborationType Type { get; set; } = CollaborationType.Private;
    public int MaxMembers { get; set; } = 10;
}

public class UpdateSpaceRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public CollaborationType? Type { get; set; }
    public int? MaxMembers { get; set; }
}

public class CreateReadingInsightRequest
{
    public string PostId { get; set; } = string.Empty;
    public int ReadingProgress { get; set; } = 0;
    public int TimeSpentSeconds { get; set; } = 0;
    public List<string> HighlightedTexts { get; set; } = new();
    public List<string> BookmarkedSections { get; set; } = new();
}

public class CreateSeriesRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
}

public class UpdateSeriesRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool? IsPublished { get; set; }
}
