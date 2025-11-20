using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models;

public class ContactMessage
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Subject { get; set; }
    
    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;
    
    public ContactStatus Status { get; set; } = ContactStatus.New;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? RespondedAt { get; set; }
    
    public string? Response { get; set; }
    
    public string? AdminNotes { get; set; }
}

public enum ContactStatus
{
    New = 0,
    InProgress = 1,
    Responded = 2,
    Closed = 3
}

