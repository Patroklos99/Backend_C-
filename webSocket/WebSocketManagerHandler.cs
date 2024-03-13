using Microsoft.AspNetCore.SignalR;

namespace Project.WebSocket;

public class WebSocketManagerHandler : Hub {
    public override async Task OnConnectedAsync()
    {
        // Custom logic when a new client connects
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Custom logic when a client disconnects
        await OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToAll(string message)
    {
        await Clients.All.SendAsync("notif", message);
    }
}