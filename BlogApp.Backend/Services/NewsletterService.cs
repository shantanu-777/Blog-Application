using System.Security.Cryptography;
using System.Text;
using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public class NewsletterService : INewsletterService
{
    private readonly SupabaseRestClient _client;
    private readonly ILogger<NewsletterService> _logger;

    public NewsletterService(SupabaseRestClient client, ILogger<NewsletterService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<NewsletterSubscription> SubscribeAsync(SubscribeNewsletterRequest request)
    {
        // Check if already subscribed
        var existing = await GetSubscriptionByEmailAsync(request.Email);
        if (existing != null && existing.IsActive)
        {
            return existing;
        }

        var subscription = new NewsletterSubscription
        {
            Email = request.Email,
            Name = request.Name,
            IsActive = true,
            SubscribedAt = DateTime.UtcNow,
            Preferences = request.Preferences ?? new List<string>(),
            UnsubscribeToken = GenerateUnsubscribeToken(request.Email)
        };

        if (existing != null)
        {
            // Reactivate existing subscription
            subscription.Id = existing.Id;
            var dto = subscription.ToDto();
            var updated = await _client.PatchAsync<SupabaseNewsletterSubscriptionDto>(
                $"newsletter_subscriptions?email=eq.{Uri.EscapeDataString(request.Email)}", dto);
            return updated?.ToModel() ?? subscription;
        }
        else
        {
            var dto = subscription.ToDto();
            var created = await _client.PostAsync<SupabaseNewsletterSubscriptionDto>("newsletter_subscriptions", dto) ?? dto;
            return created.ToModel();
        }
    }

    public async Task<bool> UnsubscribeAsync(string email, string? token = null)
    {
        var subscription = await GetSubscriptionByEmailAsync(email);
        if (subscription == null)
            return false;

        if (!string.IsNullOrEmpty(token) && subscription.UnsubscribeToken != token)
        {
            _logger.LogWarning("Invalid unsubscribe token for email: {Email}", email);
            return false;
        }

        subscription.IsActive = false;
        subscription.UnsubscribedAt = DateTime.UtcNow;

        var dto = subscription.ToDto();
        await _client.PatchAsync<SupabaseNewsletterSubscriptionDto>(
            $"newsletter_subscriptions?email=eq.{Uri.EscapeDataString(email)}", dto);
        return true;
    }

    public async Task<List<NewsletterSubscription>> GetSubscriptionsAsync(int page = 1, int pageSize = 50)
    {
        var offset = (page - 1) * pageSize;
        var subscriptions = await _client.GetAsync<SupabaseNewsletterSubscriptionDto>(
            $"newsletter_subscriptions?select=*&order=subscribed_at.desc&limit={pageSize}&offset={offset}");
        return subscriptions.Select(s => s.ToModel()).ToList();
    }

    public async Task<bool> IsSubscribedAsync(string email)
    {
        var subscription = await GetSubscriptionByEmailAsync(email);
        return subscription != null && subscription.IsActive;
    }

    private async Task<NewsletterSubscription?> GetSubscriptionByEmailAsync(string email)
    {
        var subscriptions = await _client.GetAsync<SupabaseNewsletterSubscriptionDto>(
            $"newsletter_subscriptions?select=*&email=eq.{Uri.EscapeDataString(email)}&limit=1");
        return subscriptions.FirstOrDefault()?.ToModel();
    }

    private static string GenerateUnsubscribeToken(string email)
    {
        var data = Encoding.UTF8.GetBytes($"{email}{DateTime.UtcNow:yyyyMMdd}");
        var hash = SHA256.HashData(data);
        return Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }
}

