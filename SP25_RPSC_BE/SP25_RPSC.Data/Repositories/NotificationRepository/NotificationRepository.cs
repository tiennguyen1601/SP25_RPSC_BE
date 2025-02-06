using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.NotificationRepository
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {

    }

    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly RpscContext _context;

        public NotificationRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
