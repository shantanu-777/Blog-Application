using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly List<User> _users = new();
    private readonly List<UserFollow> _follows = new();

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
        SeedData();
    }

    public async Task<User?> GetUserAsync(string id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId)
    {
        return _follows.Any(f => f.UserId == followerId && f.TargetUserId == followingId);
    }

    public async Task<User> UpdateUserAsync(string id, UpdateUserRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            throw new ArgumentException("User not found");

        if (!string.IsNullOrEmpty(request.DisplayName))
            user.DisplayName = request.DisplayName;

        if (!string.IsNullOrEmpty(request.Bio))
            user.Bio = request.Bio;

        if (!string.IsNullOrEmpty(request.Website))
            user.Website = request.Website;

        if (!string.IsNullOrEmpty(request.Twitter))
            user.Twitter = request.Twitter;

        if (!string.IsNullOrEmpty(request.LinkedIn))
            user.LinkedIn = request.LinkedIn;

        if (!string.IsNullOrEmpty(request.GitHub))
            user.GitHub = request.GitHub;

        if (!string.IsNullOrEmpty(request.AvatarUrl))
            user.AvatarUrl = request.AvatarUrl;

        _logger.LogInformation("Updated user with ID: {UserId}", user.Id);
        return user;
    }

    public async Task<bool> FollowUserAsync(string userId, string targetUserId)
    {
        if (userId == targetUserId)
            return false;

        var existingFollow = _follows.FirstOrDefault(f => f.UserId == userId && f.TargetUserId == targetUserId);
        if (existingFollow != null)
            return false;

        var follow = new UserFollow
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            TargetUserId = targetUserId,
            CreatedAt = DateTime.UtcNow
        };

        _follows.Add(follow);

        // Update follower counts
        var user = _users.FirstOrDefault(u => u.Id == userId);
        var targetUser = _users.FirstOrDefault(u => u.Id == targetUserId);

        if (user != null)
            user.FollowingCount++;

        if (targetUser != null)
            targetUser.FollowersCount++;

        _logger.LogInformation("User {UserId} followed user {TargetUserId}", userId, targetUserId);
        return true;
    }

    public async Task<bool> UnfollowUserAsync(string userId, string targetUserId)
    {
        var follow = _follows.FirstOrDefault(f => f.UserId == userId && f.TargetUserId == targetUserId);
        if (follow == null)
            return false;

        _follows.Remove(follow);

        // Update follower counts
        var user = _users.FirstOrDefault(u => u.Id == userId);
        var targetUser = _users.FirstOrDefault(u => u.Id == targetUserId);

        if (user != null)
            user.FollowingCount--;

        if (targetUser != null)
            targetUser.FollowersCount--;

        _logger.LogInformation("User {UserId} unfollowed user {TargetUserId}", userId, targetUserId);
        return true;
    }

    public async Task<List<User>> GetFollowersAsync(string userId)
    {
        var followerIds = _follows
            .Where(f => f.TargetUserId == userId)
            .Select(f => f.UserId)
            .ToList();

        return _users.Where(u => followerIds.Contains(u.Id)).ToList();
    }

    public async Task<List<User>> GetFollowingAsync(string userId)
    {
        var followingIds = _follows
            .Where(f => f.UserId == userId)
            .Select(f => f.TargetUserId)
            .ToList();

        return _users.Where(u => followingIds.Contains(u.Id)).ToList();
    }

    public async Task<List<User>> SearchUsersAsync(string query, int page = 1, int pageSize = 10)
    {
        var searchLower = query.ToLower();
        var results = _users
            .Where(u => u.IsActive &&
                       (u.Username.ToLower().Contains(searchLower) ||
                        u.DisplayName?.ToLower().Contains(searchLower) == true ||
                        u.Bio?.ToLower().Contains(searchLower) == true))
            .OrderByDescending(u => u.FollowersCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return results;
    }

    public async Task<List<User>> GetSuggestedUsersAsync(string userId, int count = 10)
    {
        var followingIds = _follows
            .Where(f => f.UserId == userId)
            .Select(f => f.TargetUserId)
            .ToHashSet();

        var suggestedUsers = _users
            .Where(u => u.Id != userId && u.IsActive && !followingIds.Contains(u.Id))
            .OrderByDescending(u => u.FollowersCount)
            .Take(count)
            .ToList();

        return suggestedUsers;
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, UserRole newRole)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return false;

        user.Role = newRole;
        _logger.LogInformation("Updated user {UserId} role to {Role}", userId, newRole);
        return true;
    }

    public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
    {
        return _users.Where(u => u.Role == role && u.IsActive).ToList();
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return false;

        user.IsActive = false;
        _logger.LogInformation("Deactivated user {UserId}", userId);
        return true;
    }

    public async Task<bool> ReactivateUserAsync(string userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return false;

        user.IsActive = true;
        _logger.LogInformation("Reactivated user {UserId}", userId);
        return true;
    }

    public async Task<PagedResult<User>> GetUsersAsync(AdminUserFilter filter)
    {
        var query = _users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLowerInvariant();
            query = query.Where(u =>
                (u.Email?.ToLowerInvariant().Contains(search) ?? false) ||
                (u.Username?.ToLowerInvariant().Contains(search) ?? false) ||
                (u.DisplayName?.ToLowerInvariant().Contains(search) ?? false));
        }

        if (filter.Role.HasValue)
        {
            query = query.Where(u => u.Role == filter.Role.Value);
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == filter.IsActive.Value);
        }

        var total = query.Count();
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<User>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return _users.ToList();
    }

    private void SeedData()
    {
        var sampleUsers = new List<User>
        {
            new User
            {
                Id = "sample-author",
                Email = "author@example.com",
                Username = "sampleauthor",
                DisplayName = "Sample Author",
                Bio = "A passionate writer and developer",
                Role = UserRole.Author,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                FollowersCount = 150,
                FollowingCount = 50,
                PostsCount = 25,
                LikesCount = 500
            },
            new User
            {
                Id = "sample-editor",
                Email = "editor@example.com",
                Username = "sampleeditor",
                DisplayName = "Sample Editor",
                Bio = "Content editor and reviewer",
                Role = UserRole.Editor,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                FollowersCount = 200,
                FollowingCount = 75,
                PostsCount = 10,
                LikesCount = 300
            },
            new User
            {
                Id = "sample-admin",
                Email = "admin@example.com",
                Username = "sampleadmin",
                DisplayName = "Sample Admin",
                Bio = "Platform administrator",
                Role = UserRole.Admin,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-90),
                FollowersCount = 500,
                FollowingCount = 100,
                PostsCount = 5,
                LikesCount = 1000
            }
        };

        _users.AddRange(sampleUsers);
    }
}

public class UserFollow
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
