using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILogger<TagController> _logger;

    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(CreateTagRequest request)
    {
        try
        {
            var tag = await _tagService.CreateTagAsync(request);
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tag");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tag>> GetTag(string id)
    {
        try
        {
            var tag = await _tagService.GetTagAsync(id);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Tag>> GetTagBySlug(string slug)
    {
        try
        {
            var tag = await _tagService.GetTagBySlugAsync(slug);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag by slug");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetTags([FromQuery] bool includeInactive = false)
    {
        try
        {
            var tags = await _tagService.GetTagsAsync(includeInactive);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tags");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Tag>> UpdateTag(string id, UpdateTagRequest request)
    {
        try
        {
            var tag = await _tagService.UpdateTagAsync(id, request);
            return Ok(tag);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tag");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
        try
        {
            var success = await _tagService.DeleteTagAsync(id);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{slug}/posts")]
    public async Task<ActionResult<List<BlogPost>>> GetPostsByTag(string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var posts = await _tagService.GetPostsByTagAsync(slug, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts by tag");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

