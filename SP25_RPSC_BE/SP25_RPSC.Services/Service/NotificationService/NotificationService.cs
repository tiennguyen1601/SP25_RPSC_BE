using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.NotificationModel.Request;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.Hubs.NotificationHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task AddNotification(Notification notification)
        {
            await _unitOfWork.NotificationRepository.Add(notification);
            await _unitOfWork.SaveAsync();
        }

        public async Task SendNotificationToUser(NotificationReqModel notification)
        {
            Notification newNotification = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                Message = notification.Message,
                UserId = notification.UserId,
            };
            await _unitOfWork.NotificationRepository.Add(newNotification);
            await _hubContext.Clients.Group(notification.UserId.ToString()).SendAsync("ReceiveNotification", JsonConvert.SerializeObject(newNotification));
            await _unitOfWork.SaveAsync();
        }

    }
}
