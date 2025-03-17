using SP25_RPSC.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.NotificationService
{
    public interface INotificationService
    {
        Task AddNotification(Notification notification);
    }
}
