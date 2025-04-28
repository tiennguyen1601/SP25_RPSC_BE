using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.BackgroundService
{
        public class PostRoomExpirationService : Microsoft.Extensions.Hosting.BackgroundService
    {
            private readonly ILogger<PostRoomExpirationService> _logger;
            private readonly IServiceProvider _serviceProvider;

            // Check interval - runs once every hour by default
            private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

            public PostRoomExpirationService(
                ILogger<PostRoomExpirationService> logger,
                IServiceProvider serviceProvider)
            {
                _logger = logger;
                _serviceProvider = serviceProvider;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("Post Room Expiration Service is starting.");

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Post Room Expiration Service is checking for expired posts.");

                    try
                    {
                        await CheckAndUpdateExpiredPosts();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while checking for expired posts.");
                    }

                    // Wait for the specified interval before checking again
                    await Task.Delay(_checkInterval, stoppingToken);
                }

                _logger.LogInformation("Post Room Expiration Service is stopping.");
            }

            private async Task CheckAndUpdateExpiredPosts()
            {
                // Create a scope to resolve scoped services
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Get all active posts
                var activePosts = await unitOfWork.PostRoomRepository.Get(
                    filter: p => p.Status == StatusEnums.Active.ToString());

                var now = DateTime.Now;
                var expiredPosts = new List<PostRoom>();

                foreach (var post in activePosts)
                {
                    // Calculate expiration date based on DateUpPost and DateExist
                    if (post.DateUpPost.HasValue && post.DateExist.HasValue)
                    {
                        var expirationDate = post.DateUpPost.Value.AddDays(post.DateExist.Value);

                        // Check if post has expired
                        if (now >= expirationDate)
                        {
                            _logger.LogInformation($"Deactivating expired post: {post.PostRoomId}, Title: {post.Title}");

                            // Update status to Inactive
                            post.Status = StatusEnums.Inactive.ToString();
                            post.UpdatedAt = now;

                            expiredPosts.Add(post);
                        }
                    }
                }

                // Update all expired posts
                if (expiredPosts.Any())
                {
                    foreach (var post in expiredPosts)
                    {
                        await unitOfWork.PostRoomRepository.Update(post);
                    }

                    // Save changes
                    await unitOfWork.SaveAsync();

                    _logger.LogInformation($"Deactivated {expiredPosts.Count} expired posts.");
                }
                else
                {
                    _logger.LogInformation("No expired posts found.");
                }
            }
        }
}
