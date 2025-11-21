using Microsoft.AspNetCore.Mvc;
using BlogApp.Backend.Services;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(CreateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(string id)
    {
        try
        {
            var category = await _categoryService.GetCategoryAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Category>> GetCategoryBySlug(string slug)
    {
        try
        {
            var category = await _categoryService.GetCategoryBySlugAsync(slug);
            if (category == null)
                return NotFound();
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category by slug");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetCategories([FromQuery] bool includeInactive = false)
    {
        try
        {
            var categories = await _categoryService.GetCategoriesAsync(includeInactive);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Category>> UpdateCategory(string id, UpdateCategoryRequest request)
    {
        try
        {
            var category = await _categoryService.UpdateCategoryAsync(id, request);
            return Ok(category);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(string id)
    {
        try
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{slug}/posts")]
    public async Task<ActionResult<List<BlogPost>>> GetPostsByCategory(string slug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var posts = await _categoryService.GetPostsByCategoryAsync(slug, page, pageSize);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts by category");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

