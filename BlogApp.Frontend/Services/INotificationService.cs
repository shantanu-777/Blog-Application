using Microsoft.AspNetCore.SignalR.Client;

namespace BlogApp.Frontend.Services;

public interface INotificationService
{
    Task InitializeAsync();
    Task SendNotificationAsync(string userId, string message);
    Task SendGroupNotificationAsync(string groupName, string message);
    Task JoinGroupAsync(string groupName);
    Task LeaveGroupAsync(string groupName);
    event Action<string>? NotificationReceived;
    event Action<string>? GroupNotificationReceived;
}
