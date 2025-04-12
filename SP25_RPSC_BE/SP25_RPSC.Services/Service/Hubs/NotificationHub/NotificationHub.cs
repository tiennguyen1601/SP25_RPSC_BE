using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.Hubs.NotificationHub
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public async Task SendNotification(string userId, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(message))
                {
                    _logger.LogWarning("SendNotification called with empty userId or message.");
                    throw new HubException("Invalid parameters.");
                }

                _logger.LogInformation($"Preparing to send notification to user {userId} with message: {message}");

                var result = Clients.Group(userId);
                if (result != null)
                {
                    await result.SendAsync("ReceiveNotification", message);
                    _logger.LogInformation($"Notification sent successfully to user {userId}: {message}");
                }
                else
                {
                    _logger.LogWarning($"Group {userId} not found or has no connected clients.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification to user {userId}");
                throw;
            }
        }

        public async Task JoinNotificationGroup(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new HubException("Invalid userId.");
                }

                _logger.LogInformation($"Client {Context.ConnectionId} attempting to join notification group {userId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation($"Client {Context.ConnectionId} successfully joined notification group {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in JoinNotificationGroup");
                throw;
            }
        }

        public async Task LeaveNotificationGroup(string userId)
        {
            try
            {
                _logger.LogInformation($"Client {Context.ConnectionId} leaving notification group {userId}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation($"Client {Context.ConnectionId} left notification group {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LeaveNotificationGroup");
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected with error");
            }
        }
    }
}
