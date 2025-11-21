using System.Net.Http.Json;

namespace BlogApp.Frontend.Services;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SubmitContactFormAsync(string name, string email, string? subject, string message)
    {
        try
        {
            var request = new { Name = name, Email = email, Subject = subject, Message = message };
            var response = await _httpClient.PostAsJsonAsync("api/contact", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

