using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface IContactService
{
    Task<ContactMessage> SubmitContactFormAsync(SubmitContactRequest request);
    Task<ContactMessage?> GetContactMessageAsync(string id);
    Task<List<ContactMessage>> GetContactMessagesAsync(ContactFilter filter);
    Task<ContactMessage> UpdateContactStatusAsync(string id, UpdateContactStatusRequest request);
}

public class SubmitContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ContactFilter
{
    public ContactStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class UpdateContactStatusRequest
{
    public ContactStatus Status { get; set; }
    public string? Response { get; set; }
    public string? AdminNotes { get; set; }
}

