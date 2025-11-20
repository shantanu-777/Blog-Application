using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(string id)
    {
        try
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("username/{username}")]
    public async Task<ActionResult<User>> GetUserByUsername(string username)
    {
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/isfollowing/{followerId}")]
    public async Task<ActionResult<bool>> IsFollowing(string id, string followerId)
    {
        try
        {
            var isFollowing = await _userService.IsFollowingAsync(followerId, id);
            return Ok(isFollowing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if following");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("current")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userService.GetUserAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(string id, UpdateUserRequest request)
    {
        try
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || currentUserId != id)
                return Forbid();

            var user = await _userService.UpdateUserAsync(id, request);
            return Ok(user);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/follow")]
    public async Task<ActionResult> FollowUser(string id)
    {
        try
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var success = await _userService.FollowUserAsync(currentUserId, id);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}/follow")]
    public async Task<ActionResult> UnfollowUser(string id)
    {
        try
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var success = await _userService.UnfollowUserAsync(currentUserId, id);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/followers")]
    public async Task<ActionResult<List<User>>> GetFollowers(string id)
    {
        try
        {
            var followers = await _userService.GetFollowersAsync(id);
            return Ok(followers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/following")]
    public async Task<ActionResult<List<User>>> GetFollowing(string id)
    {
        try
        {
            var following = await _userService.GetFollowingAsync(id);
            return Ok(following);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<User>>> SearchUsers([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var users = await _userService.SearchUsersAsync(query, page, pageSize);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/suggested")]
    public async Task<ActionResult<List<User>>> GetSuggestedUsers(string id, [FromQuery] int count = 10)
    {
        try
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || currentUserId != id)
                return Forbid();

            var users = await _userService.GetSuggestedUsersAsync(id, count);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suggested users");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}/role")]
    public async Task<ActionResult> UpdateUserRole(string id, [FromBody] UpdateUserRoleRequest request)
    {
        try
        {
            var success = await _userService.UpdateUserRoleAsync(id, request.Role);
            return success ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("by-role/{role}")]
    public async Task<ActionResult<List<User>>> GetUsersByRole(UserRole role)
    {
        try
        {
            var users = await _userService.GetUsersByRoleAsync(role);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult> DeactivateUser(string id)
    {
        try
        {
            var success = await _userService.DeactivateUserAsync(id);
            return success ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}/reactivate")]
    public async Task<ActionResult> ReactivateUser(string id)
    {
        try
        {
            var success = await _userService.ReactivateUserAsync(id);
            return success ? Ok() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating user");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
