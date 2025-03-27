using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.Hubs.ChatHub
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new();
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinChat(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("User ID is required.");
            }

            _logger.LogInformation($"User {userId} joined with ConnectionId: {Context.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await Clients.All.SendAsync("ReceiveMessage", "System", $"User {userId} has joined the chat.");
        }

        public async Task SendMessageToUser(string receiverId, string senderId, string message)
        {
            _logger.LogInformation($"Message from {senderId} to {receiverId}: {message}");

            await Clients.Group(receiverId).SendAsync("ReceiveMessage", senderId, receiverId, message);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string senderId, string receiverId)
        {
            var groupName = $"{senderId}_{receiverId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedRoom", groupName);
        }

        public async Task LeaveRoom(string senderId, string receiverId)
        {
            var groupName = $"{senderId}_{receiverId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessageToRoom(string groupName, string senderId, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
