using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models;

public class Tag
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    public string? Slug { get; set; }
    
    public string? Color { get; set; }
    
    public int PostsCount { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
}

