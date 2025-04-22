using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.UnitOfWorks;



namespace SP25_RPSC.Services.Service.BackgroundService
{
    public class ContractAutoDeactivateService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ILogger<ContractAutoDeactivateService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ContractAutoDeactivateService(
            ILogger<ContractAutoDeactivateService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ContractAutoDeactivateService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var expiringContracts = (await unitOfWork.CustomerContractRepository.Get(
                        filter: c => c.Status == StatusEnums.Active.ToString()
                            && c.EndDate.HasValue
                            && c.EndDate.Value <= DateTime.UtcNow.AddDays(7)
                    )).ToList();

                    foreach (var contract in expiringContracts)
                    {
                        var latestRequest = (await unitOfWork.ExtendContractRequestRepository.Get(
                            filter: r => r.ContractId == contract.ContractId
                        )).OrderByDescending(r => r.CreatedAt).FirstOrDefault();

                        if (latestRequest != null && latestRequest.Status == StatusEnums.Reject.ToString())
                        {
                            // 1. Inactive hợp đồng
                            contract.Status = StatusEnums.Inactive.ToString();
                            unitOfWork.CustomerContractRepository.Update(contract);
                            _logger.LogInformation($"Contract {contract.ContractId} set to Inactive.");

                            // 2. Tìm RoomStay theo RentalRoomId
                            var roomStay = (await unitOfWork.RoomStayRepository.Get(
                                filter: rs => rs.RoomId == contract.RentalRoomId
                            )).FirstOrDefault();

                            if (roomStay != null)
                            {
                                // 3. Inactive tất cả RoomStayCustomer thuộc RoomStay này
                                var roomMembers = await unitOfWork.RoomStayCustomerRepository.Get(
                                    filter: rsc => rsc.RoomStayId == roomStay.RoomStayId
                                        && rsc.Status == StatusEnums.Active.ToString()
                                );

                                foreach (var member in roomMembers)
                                {
                                    member.Status = StatusEnums.Inactive.ToString();
                                    member.UpdatedAt = DateTime.UtcNow;
                                    unitOfWork.RoomStayCustomerRepository.Update(member);
                                    _logger.LogInformation($"RoomStayCustomer {member.RoomStayCustomerId} set to Inactive.");
                                }
                            }

                            // 4. Inactive luôn phòng
                            var room = await unitOfWork.RoomRepository.GetByIDAsync(contract.RentalRoomId);
                            if (room != null && room.Status == StatusEnums.Active.ToString())
                            {
                                room.Status = StatusEnums.Inactive.ToString();
                                room.UpdatedAt = DateTime.UtcNow;
                                unitOfWork.RoomRepository.Update(room);
                                _logger.LogInformation($"Room {room.RoomId} set to Inactive.");
                            }
                        }
                    }

                    await unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ContractAutoDeactivateService.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }


    }
}