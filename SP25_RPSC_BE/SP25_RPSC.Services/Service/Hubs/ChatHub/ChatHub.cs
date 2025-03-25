using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.Hubs.ChatHub
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new();
        private readonly ILogger<ChatHub> _logger;
        //private readonly SharedDB _sharedDB;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
            //_sharedDB = sharedDB;
        }

        public async Task JoinChat(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new HubException("User name is required.");
            }

            //_sharedDB.connections[Context.ConnectionId] = userName;
            _logger.LogInformation($"User {userName} joined with ConnectionId: {Context.ConnectionId}");

            // Notify all other clients (admins or users)
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{userName} has joined the chat with ConnectionId: {Context.ConnectionId}");
        }

        public async Task SendMessage(string connectionId, string senderName, string message)
        {
            //if (!_sharedDB.connections.ContainsKey(connectionId))
            //{
            //    throw new HubException("The specified recipient does not exist.");
            //}

            // Log the message for debugging
            _logger.LogInformation($"Message from {senderName} to {connectionId}: {message}");

            // Send the message to the recipient
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderName, message);
            await Clients.Caller.SendAsync("ReceiveMessage", senderName, message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //if (_sharedDB.connections.TryRemove(Context.ConnectionId, out string groupName))
            //{
            //    //_logger.LogInformation($"User {groupName} with ConnectionId {Context.ConnectionId} disconnected.");
            //    //await Clients.Others.SendAsync("ReceiveMessage", "System", $"{userName} has left the chat.");

            //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            //}

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string senderName, string receiverName)
        {
            var groupName = $"{senderName}_{receiverName}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //_sharedDB.connections[Context.ConnectionId] = groupName;

            // Notify the user/admin they've joined the group
            await Clients.Caller.SendAsync("JoinedRoom", groupName);
        }

        public async Task LeaveRoom(string adminName, string userName)
        {
            var groupName = $"{userName}_{adminName}";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessageToRoom(string groupName, string sender, string message)
        {
            // Broadcast to the group only
            await Clients.Group(groupName).SendAsync("ReceiveMessage", sender, message);
        }

    }

}
