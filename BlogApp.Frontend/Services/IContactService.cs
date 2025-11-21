namespace BlogApp.Frontend.Services;

public interface IContactService
{
    Task<bool> SubmitContactFormAsync(string name, string email, string? subject, string message);
}

