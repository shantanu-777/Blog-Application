using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface IUserService
{
    Task<User?> GetUserAsync(string id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> UpdateUserAsync(string id, UpdateUserRequest request);
    Task<bool> FollowUserAsync(string userId, string targetUserId);
    Task<bool> UnfollowUserAsync(string userId, string targetUserId);
    Task<bool> IsFollowingAsync(string followerId, string followingId);
    Task<List<User>> GetFollowersAsync(string userId);
    Task<List<User>> GetFollowingAsync(string userId);
    Task<List<User>> SearchUsersAsync(string query, int page = 1, int pageSize = 10);
    Task<List<User>> GetSuggestedUsersAsync(string userId, int count = 10);
    Task<bool> UpdateUserRoleAsync(string userId, UserRole newRole);
    Task<List<User>> GetUsersByRoleAsync(UserRole role);
    Task<bool> DeactivateUserAsync(string userId);
    Task<bool> ReactivateUserAsync(string userId);
    Task<PagedResult<User>> GetUsersAsync(AdminUserFilter filter);
    Task<List<User>> GetAllUsersAsync();
}

public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? Website { get; set; }
    public string? Twitter { get; set; }
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }
    public string? AvatarUrl { get; set; }
}
