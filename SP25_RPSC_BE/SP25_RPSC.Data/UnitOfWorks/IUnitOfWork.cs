using Microsoft.EntityFrameworkCore.Storage;
using SP25_RPSC.Data.Repositories.AddressRepository;
using SP25_RPSC.Data.Repositories.BussinessImageRepository;
using SP25_RPSC.Data.Repositories.CustomerContractRepository;
using SP25_RPSC.Data.Repositories.CustomerRepository;
using SP25_RPSC.Data.Repositories.CustomerRequestRepository;
using SP25_RPSC.Data.Repositories.FavoriteRepository;
using SP25_RPSC.Data.Repositories.FeedbackRepository;
using SP25_RPSC.Data.Repositories.LandlordContractRepository;
using SP25_RPSC.Data.Repositories.LandlordRepository;
using SP25_RPSC.Data.Repositories.NotificationRepository;
using SP25_RPSC.Data.Repositories.OTPRepository;
using SP25_RPSC.Data.Repositories.PaymentRepository;
using SP25_RPSC.Data.Repositories.PostRepository;
using SP25_RPSC.Data.Repositories.PricePackageRepository;
using SP25_RPSC.Data.Repositories.RefreshTokenRepository;
using SP25_RPSC.Data.Repositories.ReportRepository;
using SP25_RPSC.Data.Repositories.RoleRepository;
using SP25_RPSC.Data.Repositories.RoomImageRepository;
using SP25_RPSC.Data.Repositories.RoommateRequestRepository;
using SP25_RPSC.Data.Repositories.RoomPriceRepository;
using SP25_RPSC.Data.Repositories.RoomRepository;
using SP25_RPSC.Data.Repositories.RoomServicePriceRepository;
using SP25_RPSC.Data.Repositories.RoomServiceRepository;
using SP25_RPSC.Data.Repositories.RoomStayCustomerRepository;
using SP25_RPSC.Data.Repositories.RoomStayRepository;
using SP25_RPSC.Data.Repositories.RoomTypeRepository;
using SP25_RPSC.Data.Repositories.ServiceDetailRepository;
using SP25_RPSC.Data.Repositories.ServicePackageRepository;
using SP25_RPSC.Data.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IAddressRepository AddressRepository { get; }
        ICustomerContractRepository CustomerContractRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        ICustomerRequestRepository CustomerRequestRepository { get; }
        IFavoriteRepository FavoriteRepository { get; }
        IFeedbackRepository FeedbackRepository { get; }
        ILandlordContractRepository LandlordContractRepository { get; }
        ILandlordRepository LandlordRepository { get; }
        INotificationRepository NotificationRepository { get; }
        IPostRepository PostRepository { get; }
        IPricePackageRepository PricePackageRepository { get; }
        IReportRepository ReportRepository { get; }
        IRoleRepository RoleRepository { get; }
        IRoomImageRepository RoomImageRepository { get; }
        IRoommateRequestRepository RoommateRequestRepository { get; }
        IRoomPriceRepository RoomPriceRepository { get; }
        IRoomRepository RoomRepository { get; }
        IRoomServicePriceRepository RoomServicePriceRepository { get; }
        IRoomServiceRepository RoomServiceRepository { get; }
        IRoomStayCustomerRepository RoomStayCustomerRepository { get; }
        IRoomStayRepository RoomStayRepository { get; }
        IRoomTypeRepository RoomTypeRepository { get; }
        IServicePackageRepository ServicePackageRepository { get; }
        IServiceDetailRepository ServiceDetailRepository { get; }
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        ITransactionRepository TransactionRepository { get; }

        IOTPRepository OTPRepository { get; }

        IBussinessImageRepository BussinessImageRepository { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task SaveAsync();
        void Save();
    }
}
