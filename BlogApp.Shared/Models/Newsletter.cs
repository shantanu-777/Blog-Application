using System.ComponentModel.DataAnnotations;

namespace BlogApp.Shared.Models;

public class NewsletterSubscription
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? Name { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UnsubscribedAt { get; set; }
    
    public string? UnsubscribeToken { get; set; }
    
    public List<string> Preferences { get; set; } = new();
}

