using Microsoft.EntityFrameworkCore.Storage;
using SP25_RPSC.Data.Repositories;
using SP25_RPSC.Data.Repositories.AddressRepository;
using SP25_RPSC.Data.Repositories.CustomerContractRepository;
using SP25_RPSC.Data.Repositories.CustomerRepository;
using SP25_RPSC.Data.Repositories.CustomerRequestRepository;
using SP25_RPSC.Data.Repositories.ExtendContractRepository;
using SP25_RPSC.Data.Repositories.FavoriteRepository;
using SP25_RPSC.Data.Repositories.FeedbackRepository;
using SP25_RPSC.Data.Repositories.LandlordContractRepository;
using SP25_RPSC.Data.Repositories.LandlordRepository;
using SP25_RPSC.Data.Repositories.NotificationRepository;
using SP25_RPSC.Data.Repositories.PaymentRepository;
using SP25_RPSC.Data.Repositories.PostRepository;
using SP25_RPSC.Data.Repositories.PricePackageRepository;
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
using SP25_RPSC.Data.Repositories.ServicePackageRepository;
using SP25_RPSC.Data.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private RpscContext _context;
        private IAddressRepository _addressRepository;
        private ICustomerContractRepository _customerContractRepository;
        private ICustomerRepository _customerRepository;
        private ICustomerRequestRepository _customerRequestRepository;
        private IExtendContractRepository _extendContractRepository;
        private IFavoriteRepository _favoriteRepository;
        private IFeedbackRepository _feedbackRepository;
        private ILandlordContractRepository _landlordContractRepository;
        private ILandlordRepository _landlordRepository;
        private INotificationRepository _notificationRepository;
        private IPaymentRepository _paymentRepository;
        private IPostRepository _postRepository;
        private IPricePackageRepository _pricePackageRepository;
        private IReportRepository _reportRepository;
        private IRoleRepository _roleRepository;
        private IRoomImageRepository _roomImageRepository;
        private IRoommateRequestRepository _roommateRequestRepository;
        private IRoomPriceRepository _roomPriceRepository;
        private IRoomRepository _roomRepository;
        private IRoomServicePriceRepository _roomServicePriceRepository;
        private IRoomServiceRepository _roomServiceRepository;
        private IRoomStayCustomerRepository _roomStayCustomerRepository;
        private IRoomStayRepository _roomStayRepository;
        private IRoomTypeRepository _roomTypeRepository;
        private IServicePackageRepository _servicePackageRepository;
        private IUserRepository _usersRepository;
        

        public UnitOfWork(RpscContext context)
        {
            _context = context;
        }
        public IAddressRepository AddressRepository
        {
            get
            {
                return _addressRepository ??= new AddressRepository(_context);
            }
        }

        public ICustomerContractRepository CustomerContractRepository {
            get
            {
                return _customerContractRepository ??= new CustomerContractRepository(_context);
            }
        }

        public ICustomerRepository CustomerRepository {
            get
            {
                return _customerRepository ??= new CustomerRepository(_context);
            }
        }

        public ICustomerRequestRepository CustomerRequestRepository {
            get
            {
                return _customerRequestRepository ??= new CustomerRequestRepository(_context);
            }
        }

        public IExtendContractRepository ExtendContractRepository {
            get
            {
                return _extendContractRepository ??= new ExtendContractRepository(_context);
            }
        }

        public IFavoriteRepository FavoriteRepository{
            get
            {
                return _favoriteRepository ??= new FavoriteRepository(_context);
            }
        }

        public IFeedbackRepository FeedbackRepository {
            get
            {
                return _feedbackRepository ??= new FeedbackRepository(_context);
            }
        }

        public ILandlordContractRepository LandlordContractRepository {
            get
            {
                return _landlordContractRepository ??= new LandlordContractRepository(_context);
            }
        }

        public ILandlordRepository LandlordRepository {
            get
            {
                return _landlordRepository ??= new LandlordRepository(_context);
            }
        }

        public INotificationRepository NotificationRepository {
            get
            {
                return _notificationRepository ??= new NotificationRepository(_context);
            }
        }

        public IPaymentRepository PaymentRepository {
            get
            {
                return _paymentRepository ??= new PaymentRepository(_context);
            }
        }

        public IPostRepository PostRepository {
            get
            {
                return _postRepository ??= new PostRepository(_context);
            }
        }

        public IPricePackageRepository PricePackageRepository {
            get
            {
                return _pricePackageRepository ??= new PricePackageRepository(_context);
            }
        }

        public IReportRepository ReportRepository {
            get
            {
                return _reportRepository ??= new ReportRepository(_context);
            }
        }

        public IRoleRepository RoleRepository {
            get
            {
                return _roleRepository ??= new RoleRepository(_context);
            }
        }

        public IRoomImageRepository RoomImageRepository {
            get
            {
                return _roomImageRepository ??= new RoomImageRepository(_context);
            }
        }

        public IRoommateRequestRepository RoommateRequestRepository {
            get
            {
                return _roommateRequestRepository ??= new RoommateRequestRepository(_context);
            }
        }

        public IRoomPriceRepository RoomPriceRepository {
            get
            {
                return _roomPriceRepository ??= new RoomPriceRepository(_context);
            }
        }

        public IRoomRepository RoomRepository {
            get
            {
                return _roomRepository ??= new RoomRepository(_context);
            }
        }

        public IRoomServicePriceRepository RoomServicePriceRepository {
            get
            {
                return _roomServicePriceRepository ??= new RoomServicePriceRepository(_context);
            }
        }

        public IRoomServiceRepository RoomServiceRepository {
            get
            {
                return _roomServiceRepository ??= new RoomServiceRepository(_context);
            }
        }

        public IRoomStayCustomerRepository RoomStayCustomerRepository {
            get
            {
                return _roomStayCustomerRepository ??= new RoomStayCustomerRepository(_context);
            }
        }

        public IRoomStayRepository RoomStayRepository{
            get
            {
                return _roomStayRepository ??= new RoomStayRepository(_context);
            }
        }

        public IRoomTypeRepository RoomTypeRepository{
            get
            {
                return _roomTypeRepository ??= new RoomTypeRepository(_context);
            }
        }

        public IServicePackageRepository ServicePackageRepository {
            get
            {
                return _servicePackageRepository ??= new ServicePackageRepository(_context);
            }
        }

        public IUserRepository UserRepository {
            get
            {
                return _usersRepository ??= new UserRepository(_context);
            }
        }

        public void Save()
        {
            var validationErrors = _context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }
            _context.SaveChanges();
        }

        public async System.Threading.Tasks.Task SaveAsync()
        {
            var validationErrors = _context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
