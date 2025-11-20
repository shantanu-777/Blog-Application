using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Username { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? DisplayName { get; set; }
    
    [StringLength(500)]
    public string? Bio { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public string? Website { get; set; }
    
    public string? Twitter { get; set; }
    
    public string? LinkedIn { get; set; }
    
    public string? GitHub { get; set; }
    
    public UserRole Role { get; set; } = UserRole.Reader;
    
    public bool IsEmailVerified { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int FollowersCount { get; set; } = 0;
    
    public int FollowingCount { get; set; } = 0;
    
    public int PostsCount { get; set; } = 0;
    
    public int LikesCount { get; set; } = 0;
}

public enum UserRole
{
    Reader = 0,
    Author = 1,
    Editor = 2,
    Admin = 3
}
