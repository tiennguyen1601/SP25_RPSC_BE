using AutoMapper;
using CloudinaryDotNet.Actions;
using MimeKit.Cryptography;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<List<ServicePackageReponse>> GetAllServicePackage()
        {
            var servicePackages = await _unitOfWork.ServicePackageRepository.Get(orderBy: q => q.OrderBy(p => p.Duration));

            if (servicePackages == null || !servicePackages.Any())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "ServicePackageEmpty");
            }

            return _mapper.Map<List<ServicePackageReponse>>(servicePackages);
        }



        public Task<List<ServiceDetailReponse>> GetServiceDetailsByPackageId(string packageId)
        {
            var servicePackages = _unitOfWork.ServiceDetailRepository
                .Get(u => u.PackageId.Equals(packageId), orderBy: q => q.OrderBy(p => p.LimitPost), includeProperties: "PricePackages").Result 
                .ToList();

            if (!servicePackages.Any())
            {
                throw new ApiException(HttpStatusCode.NotFound, "No ServiceDetails found for the given PackageId");
            }

            var responseList = servicePackages.Select(servicePackage => new ServiceDetailReponse
            {
                ServiceDetailId = servicePackage.ServiceDetailId,
                Type = servicePackage.Type,
                LimitPost = servicePackage.LimitPost,
                Status = servicePackage.Status,
                PackageId = servicePackage.PackageId,
                Price = servicePackage.PricePackages?.FirstOrDefault()?.Price ?? 0,
                ApplicableDate = servicePackage.PricePackages?.FirstOrDefault()?.ApplicableDate
            }).ToList();

            return Task.FromResult(responseList);
        }
        public async Task<List<ServicePackageLandlordResponse>> GetServicePackageForLanlord()
        {
            var servicePackages = await _unitOfWork.ServicePackageRepository
                .Get(orderBy: q => q.OrderBy(p => p.Duration), includeProperties: "ServiceDetails.PricePackages");

            if (servicePackages == null || !servicePackages.Any())
            {
                throw new ApiException(HttpStatusCode.NotFound, "No Service Packages found for landlords");
            }

            var responseList = servicePackages.Select(package => new ServicePackageLandlordResponse
            {
                PackageId = package.PackageId,
                Name = package.Name,
                Duration = package.Duration,
                Description = package.Description,
                Status = package.Status,
                ListServicePrice = package.ServiceDetails?.Select(serviceDetail => new ServicePriceResponse
                {
                    ServiceDetailId = serviceDetail.ServiceDetailId,
                    Type = serviceDetail.Type,
                    LimitPost = serviceDetail.LimitPost,
                    PriceId = serviceDetail.PricePackages?.FirstOrDefault()?.PriceId ?? string.Empty,
                    Price = serviceDetail.PricePackages?.FirstOrDefault()?.Price ?? 0
                })
                .OrderBy(servicePrice => servicePrice.Price)
                .ToList() ?? new List<ServicePriceResponse>()
            }).ToList();

            return responseList;
        }

    }
}
