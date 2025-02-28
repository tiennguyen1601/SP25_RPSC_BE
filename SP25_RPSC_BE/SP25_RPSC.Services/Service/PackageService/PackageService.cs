using AutoMapper;
using CloudinaryDotNet.Actions;
using MimeKit.Cryptography;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PackageService
{
    public class PackageService : IPackageService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreatePackage(PackageCreateRequestModel model)
        {
            if (model.PackageDetails == null || !model.PackageDetails.Any())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "PackageDetailsEmpty");
            }

            var packageDetails = _mapper.Map<List<ServiceDetail>>(model.PackageDetails);

            var pricePackages = model.PackageDetails
                .Select(d => d.PricePackageModel)
                .Where(p => p != null)
                .ToList();

            if (!pricePackages.Any())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "PricePackageEmty");
            }     

            var package = new ServicePackage
            {
                Name = model.Name,
                Description = model.Description,
                Duration = model.Duration,
                ServiceDetails = packageDetails,
                Status = StatusEnums.Active.ToString(),
            };

            //foreach (var serviceDetail in package.ServiceDetails)
            //{
            //    serviceDetail.PricePackages = (ICollection<PricePackage>)pricePackages;
            //}

            await _unitOfWork.ServicePackageRepository.Add(package);
        }

        public async Task<List<ServicePackage>> GetAllServicePackage()
        {
            var servicePackages = await _unitOfWork.ServicePackageRepository.GetAll();
            if (servicePackages == null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "ServicePackageEmty");
            }
            return servicePackages;
        }

        public async Task<ServicePackage> GetServicePackageById(string id)
        {
            var servicePackage = await _unitOfWork.ServicePackageRepository.GetServicePackageById(id);
            if(servicePackage == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "ServicePackageNotFound");
            }
            return servicePackage;
        }
    }
}
