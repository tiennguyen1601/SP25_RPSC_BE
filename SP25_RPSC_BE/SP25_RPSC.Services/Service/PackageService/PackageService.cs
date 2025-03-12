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



        public async Task<ServiceDetailReponse?> GetServiceDetailsByPackageId(string packageId)
        {
            var servicePackage = (await _unitOfWork.ServicePackageRepository
                .Get(sp => sp.PackageId == packageId, includeProperties: "ServiceDetails.PricePackages")).FirstOrDefault();

            if (servicePackage == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "No ServiceDetails found for the given PackageId");
            }
            var response = new ServiceDetailReponse
            {
                PackageId = servicePackage.PackageId,
                Name = servicePackage.Name, 
                Duration = servicePackage.Duration,
                Description = servicePackage.Description,
                serviceStatus = servicePackage.Status,
                ListDetails = servicePackage.ServiceDetails.Select(detail => new ServiceDetailReponse.ListDetailService
                {
                    ServiceDetailId = detail.ServiceDetailId,
                    Type = detail.Type,
                    LimitPost = detail.HighLight,
                    Status = detail.Status,
                    PriceId = detail.PricePackages?.FirstOrDefault(p => p.Status == StatusEnums.Active.ToString())?.PriceId,
                    Price = detail.PricePackages?.FirstOrDefault(p => p.Status == StatusEnums.Active.ToString())?.Price ?? 0,
                    ApplicableDate = detail.PricePackages?.FirstOrDefault(p => p.Status == StatusEnums.Active.ToString())?.ApplicableDate
                })
                
                .OrderBy(x => x.Price).ToList()
            };

            return response;
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
                    LimitPost = serviceDetail.HighLight,
                    PriceId = serviceDetail.PricePackages?.FirstOrDefault(p => p.Status == StatusEnums.Active.ToString())?.PriceId,
                    Price = serviceDetail.PricePackages?.FirstOrDefault(p => p.Status == StatusEnums.Active.ToString())?.Price ?? 0,
                    ApplicableDate = serviceDetail.PricePackages?.FirstOrDefault(p => p.Status == StatusEnums.Active.ToString())?.ApplicableDate
                })
                .OrderBy(servicePrice => servicePrice.Price)
                .ToList() ?? new List<ServicePriceResponse>()
            }).ToList();

            return responseList;
        }


        public async Task UpdatePrice(string PriceId, decimal newPrice)
        {
            var pricePackage = await _unitOfWork.PricePackageRepository.GetByIDAsync(PriceId);

            if (pricePackage == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "No pricePackage found for the given PriceId");
            }

            pricePackage.Status = StatusEnums.Inactive.ToString();
            await _unitOfWork.PricePackageRepository.Update(pricePackage);


            var newPricePackage = new PricePackage
            {
                PriceId = Guid.NewGuid().ToString(), 
                Price = newPrice,
                Status = StatusEnums.Active.ToString(),
                ApplicableDate = DateTime.UtcNow, 
                ServiceDetailId = pricePackage.ServiceDetailId
            };

            await _unitOfWork.PricePackageRepository.Add(newPricePackage);
            await _unitOfWork.SaveAsync();
        }




    }
}
