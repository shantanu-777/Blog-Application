using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public class ContactService : IContactService
{
    private readonly SupabaseRestClient _client;
    private readonly ILogger<ContactService> _logger;

    public ContactService(SupabaseRestClient client, ILogger<ContactService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ContactMessage> SubmitContactFormAsync(SubmitContactRequest request)
    {
        var message = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            Status = ContactStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        var dto = message.ToDto();
        var created = await _client.PostAsync<SupabaseContactMessageDto>("contact_messages", dto) ?? dto;
        return created.ToModel();
    }

    public async Task<ContactMessage?> GetContactMessageAsync(string id)
    {
        var messages = await _client.GetAsync<SupabaseContactMessageDto>(
            $"contact_messages?select=*&id=eq.{Uri.EscapeDataString(id)}&limit=1");
        return messages.FirstOrDefault()?.ToModel();
    }

    public async Task<List<ContactMessage>> GetContactMessagesAsync(ContactFilter filter)
    {
        var query = "contact_messages?select=*&order=created_at.desc";
        
        if (filter.Status.HasValue)
        {
            query += $"&status=eq.{(int)filter.Status.Value}";
        }

        var offset = (filter.Page - 1) * filter.PageSize;
        query += $"&limit={filter.PageSize}&offset={offset}";

        var messages = await _client.GetAsync<SupabaseContactMessageDto>(query);
        return messages.Select(m => m.ToModel()).ToList();
    }

    public async Task<ContactMessage> UpdateContactStatusAsync(string id, UpdateContactStatusRequest request)
    {
        var existing = await GetContactMessageAsync(id) ?? throw new ArgumentException("Contact message not found");

        existing.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Response))
        {
            existing.Response = request.Response;
            existing.RespondedAt = DateTime.UtcNow;
        }
        if (!string.IsNullOrEmpty(request.AdminNotes))
        {
            existing.AdminNotes = request.AdminNotes;
        }

        var dto = existing.ToDto();
        var updated = await _client.PatchAsync<SupabaseContactMessageDto>(
            $"contact_messages?id=eq.{Uri.EscapeDataString(id)}", dto);
        return updated?.ToModel() ?? existing;
    }
}

