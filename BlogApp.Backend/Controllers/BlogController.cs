using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;
using ArchiveEntry = BlogApp.Backend.Services.ArchiveEntry;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly ILogger<BlogController> _logger;

    public BlogController(IBlogService blogService, ILogger<BlogController> logger)
    {
        _blogService = blogService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BlogPost>> CreatePost(CreatePostRequest request)
    {
        try
        {
            request.AuthorId = ResolveUserId(request.AuthorId);
            if (string.IsNullOrWhiteSpace(request.AuthorId))
                return BadRequest(new { message = "Author information is required." });

            var post = await _blogService.CreatePostAsync(request);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlogPost>> GetPost(string id)
    {
        try
        {
            var post = await _blogService.GetPostAsync(id);
            if (post == null)
                return NotFound();

            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<BlogPost>> GetPostBySlug(string slug)
    {
        try
        {
            var post = await _blogService.GetPostBySlugAsync(slug);
            if (post == null)
                return NotFound();

            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post by slug");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<BlogPost>>> GetPosts([FromQuery] GetPostsRequest request)
    {
        try
        {
            var posts = await _blogService.GetPostsAsync(request);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BlogPost>> UpdatePost(string id, UpdatePostRequest request)
    {
        try
        {
            var post = await _blogService.UpdatePostAsync(id, request);
            return Ok(post);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(string id)
    {
        try
        {
            var success = await _blogService.DeletePostAsync(id);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/like")]
    public async Task<ActionResult> LikePost(string id, [FromBody] UserActionRequest request)
    {
        try
        {
            var userId = ResolveUserId(request.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User context missing." });

            var success = await _blogService.LikePostAsync(id, userId);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error liking post");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}/like")]
    public async Task<ActionResult> UnlikePost(string id, [FromBody] UserActionRequest request)
    {
        try
        {
            var userId = ResolveUserId(request.UserId);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User context missing." });

            var success = await _blogService.UnlikePostAsync(id, userId);
            return success ? Ok() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unliking post");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<List<Comment>>> GetComments(string id)
    {
        try
        {
            var comments = await _blogService.GetCommentsAsync(id);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/comments")]
    public async Task<ActionResult<Comment>> AddComment(string id, AddCommentRequest request)
    {
        try
        {
            request.PostId = id;
            request.AuthorId = ResolveUserId(request.AuthorId);
            if (string.IsNullOrWhiteSpace(request.AuthorId))
                return Unauthorized(new { message = "User context missing." });

            var comment = await _blogService.AddCommentAsync(request);
            return CreatedAtAction(nameof(GetComments), new { id }, comment);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Commenting disabled or blocked for post {PostId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("comments/{commentId}")]
    public async Task<ActionResult<Comment>> UpdateComment(string commentId, UpdateCommentRequest request)
    {
        try
        {
            var comment = await _blogService.UpdateCommentAsync(commentId, request);
            return Ok(comment);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("comments/{commentId}")]
    public async Task<ActionResult> DeleteComment(string commentId)
    {
        try
        {
            var success = await _blogService.DeleteCommentAsync(commentId);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<BlogPost>>> GetTrendingPosts([FromQuery] int count = 10)
    {
        try
        {
            var posts = await _blogService.GetTrendingPostsAsync(count);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trending posts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("recommended")]
    public async Task<ActionResult<List<BlogPost>>> GetRecommendedPosts([FromQuery] int count = 10, [FromQuery] string? userId = null)
    {
        try
        {
            var resolved = ResolveUserId(userId);
            if (string.IsNullOrEmpty(resolved))
                return Unauthorized(new { message = "User context missing." });

            var posts = await _blogService.GetRecommendedPostsAsync(resolved, count);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommended posts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<BlogPost>>> SearchPosts([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var posts = await _blogService.SearchPostsAsync(query, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching posts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/related")]
    public async Task<ActionResult<List<BlogPost>>> GetRelatedPosts(string id, [FromQuery] int count = 3)
    {
        try
        {
            var posts = await _blogService.GetRelatedPostsAsync(id, count);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting related posts");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/liked")]
    public async Task<ActionResult<bool>> IsPostLiked(string id, [FromQuery] string? userId = null)
    {
        try
        {
            var resolved = ResolveUserId(userId);
            if (string.IsNullOrEmpty(resolved))
                return Unauthorized(new { message = "User context missing." });

            var isLiked = await _blogService.IsPostLikedAsync(id, resolved);
            return Ok(isLiked);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if post is liked");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("archive")]
    public async Task<ActionResult<List<ArchiveEntry>>> GetArchive()
    {
        try
        {
            var entries = await _blogService.GetArchiveEntriesAsync();
            return Ok(entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting archive entries");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("archive/{year}")]
    public async Task<ActionResult<List<BlogPost>>> GetPostsByYear(int year, [FromQuery] int? month = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var posts = await _blogService.GetPostsByArchiveAsync(year, month, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts by archive");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    private string? ResolveUserId(string? explicitUserId = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitUserId))
            return explicitUserId;

        if (Request.Headers.TryGetValue("X-User-Id", out var header) && !string.IsNullOrWhiteSpace(header))
            return header.ToString();

        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrWhiteSpace(claim) ? null : claim;
    }
}

public class UserActionRequest
{
    public string? UserId { get; set; }
}
