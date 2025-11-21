using System.Net.Http.Json;

namespace BlogApp.Frontend.Services;

public class NewsletterService : INewsletterService
{
    private readonly HttpClient _httpClient;

    public NewsletterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SubscribeAsync(string email, string? name = null)
    {
        try
        {
            var request = new { Email = email, Name = name };
            var response = await _httpClient.PostAsJsonAsync("api/newsletter/subscribe", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UnsubscribeAsync(string email, string? token = null)
    {
        try
        {
            var request = new { Email = email, Token = token };
            var response = await _httpClient.PostAsJsonAsync("api/newsletter/unsubscribe", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsSubscribedAsync(string email)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/newsletter/check/{Uri.EscapeDataString(email)}");
        }
        catch
        {
            return false;
        }
    }
}

