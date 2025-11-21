namespace BlogApp.Shared.Models;

public class SiteSettings
{
    public string Id { get; set; } = string.Empty;
    
    public string SiteTitle { get; set; } = "My Blog";
    
    public string SiteDescription { get; set; } = "A modern blog application";
    
    public string SiteUrl { get; set; } = "https://myblog.com";
    
    public string? LogoUrl { get; set; }
    
    public string? FaviconUrl { get; set; }
    
    public string? ThemeColor { get; set; } = "#1976d2";
    
    public string? PrimaryColor { get; set; } = "#1976d2";
    
    public string? SecondaryColor { get; set; } = "#424242";
    
    public bool AllowComments { get; set; } = true;
    
    public bool RequireCommentApproval { get; set; } = false;
    
    public bool AllowUserRegistration { get; set; } = true;
    
    public bool RequireEmailVerification { get; set; } = true;
    
    public string? ContactEmail { get; set; }
    
    public string? SocialMediaLinks { get; set; }
    
    public string? GoogleAnalyticsId { get; set; }
    
    public string? FacebookPixelId { get; set; }
    
    public int PostsPerPage { get; set; } = 10;
    
    public bool EnableNewsletter { get; set; } = true;
    
    public bool EnableSearch { get; set; } = true;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}













