using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface INewsletterService
{
    Task<NewsletterSubscription> SubscribeAsync(SubscribeNewsletterRequest request);
    Task<bool> UnsubscribeAsync(string email, string? token = null);
    Task<List<NewsletterSubscription>> GetSubscriptionsAsync(int page = 1, int pageSize = 50);
    Task<bool> IsSubscribedAsync(string email);
}

public class SubscribeNewsletterRequest
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public List<string> Preferences { get; set; } = new();
}

