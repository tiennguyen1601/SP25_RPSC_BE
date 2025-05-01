using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SP25_RPSC.Data.Repositories;
using SP25_RPSC.Services.Service.Hubs.NotificationHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class ContractExpiryCheckService : BackgroundService
    {
        private readonly ILogger<ContractExpiryCheckService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ContractExpiryCheckService(
            ILogger<ContractExpiryCheckService> logger,
            IServiceProvider serviceProvider,
            IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Contract expiry check service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RpscContext>();

                    var now = DateTime.Now.Date;
                    var oneMonthLater = now.AddMonths(1);

                    //CustomerContracts
                    var expiredCustomerContracts = dbContext.CustomerContracts
                        .Where(c => c.EndDate != null
                            && c.EndDate.Value.Date <= oneMonthLater
                            && c.EndDate.Value.Date >= now
                            && c.Status != "Expired")
                        .ToList();

                    foreach (var contract in expiredCustomerContracts)
                    {
                        var customer = contract.Tenant;
                        if (customer?.UserId != null)
                        {
                            string message = $"Hợp đồng của bạn sẽ hết hạn vào ngày {contract.EndDate?.ToString("dd/MM/yyyy")}.";

                            await _hubContext.Clients.Group(customer.UserId)
                                .SendAsync("ReceiveNotification", message);

                            _logger.LogInformation($"Notification sent to tenant user {customer.UserId}");
                        }
                    }

                    //LandlordContracts
                    var expiredLandlordContracts = dbContext.LandlordContracts
                        .Include(c => c.Landlord)
                        .Where(c => c.EndDate.HasValue
                            && c.EndDate.Value.Date <= oneMonthLater
                            && c.EndDate.Value.Date >= now
                            && c.Status != "Expired"
                            && c.Landlord != null
                            && c.Landlord.UserId != null)
                        .ToList();

                    foreach (var contract in expiredLandlordContracts)
                    {
                        var landlord = contract.Landlord;
                        if (landlord == null || string.IsNullOrEmpty(landlord.UserId))
                            continue;

                        var userId = landlord.UserId;

                        string formattedDate = contract.EndDate?.ToString("dd/MM/yyyy") ?? "không rõ";
                        string contractMessage = $"Hợp đồng dịch vụ của bạn sẽ hết hạn vào ngày {formattedDate}.";

                        await _hubContext.Clients.Group(userId)
                            .SendAsync("ReceiveNotification", contractMessage);

                        _logger.LogInformation($"Notification sent to landlord user {userId}");

                        if (landlord.NumberRoom == 0)
                        {
                            string roomMessage = $"Số lượng bài đăng của bạn đã hết. Vui lòng gia hạn gói để tiếp tục sử dụng dịch vụ.";
                            await _hubContext.Clients.Group(userId)
                                .SendAsync("ReceiveNotification", roomMessage);

                            _logger.LogInformation($"Notification sent to landlord user {userId} (no rooms).");
                        }
                    }


                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking contract expiries.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
       }


    }



}
