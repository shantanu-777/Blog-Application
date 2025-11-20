using Microsoft.AspNetCore.SignalR.Client;
using BlogApp.Frontend.Services;

namespace BlogApp.Frontend.Services;

public class NotificationService : INotificationService
{
    private HubConnection? _hubConnection;

    public event Action<string>? NotificationReceived;
    public event Action<string>? GroupNotificationReceived;

    public async Task InitializeAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("/notificationHub")
            .Build();

        _hubConnection.On<string>("ReceiveNotification", (message) =>
        {
            NotificationReceived?.Invoke(message);
        });

        _hubConnection.On<string>("ReceiveGroupNotification", (message) =>
        {
            GroupNotificationReceived?.Invoke(message);
        });

        await _hubConnection.StartAsync();
    }

    public async Task SendNotificationAsync(string userId, string message)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendNotification", userId, message);
        }
    }

    public async Task SendGroupNotificationAsync(string groupName, string message)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("SendGroupNotification", groupName, message);
        }
    }

    public async Task JoinGroupAsync(string groupName)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("JoinGroup", groupName);
        }
    }

    public async Task LeaveGroupAsync(string groupName)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("LeaveGroup", groupName);
        }
    }
}
