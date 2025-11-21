namespace BlogApp.Frontend.Services;

public interface INewsletterService
{
    Task<bool> SubscribeAsync(string email, string? name = null);
    Task<bool> UnsubscribeAsync(string email, string? token = null);
    Task<bool> IsSubscribedAsync(string email);
}

