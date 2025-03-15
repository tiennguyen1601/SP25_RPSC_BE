using AutoMapper;
using CloudinaryDotNet.Actions;
using MimeKit.Cryptography;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
using SP25_RPSC.Data.Models.UserModels.Response;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.LandlordContractService;
using SP25_RPSC.Services.Utils.CustomException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SP25_RPSC.Services.Service.PackageService
{
    public class PackageService : IPackageService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private ILandlordContractService _landlordContractService;

        public PackageService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILandlordContractService landlordContractService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _landlordContractService = landlordContractService;
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
                Type = model.Type,
                HighLight = model.HighLight,
                Size = model.Size,
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
            var servicePackages = await _unitOfWork.ServicePackageRepository.Get();

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
                Type = servicePackage.Type,
                HighLight = servicePackage.HighLight,
                Size = servicePackage.Size,
                Status = servicePackage.Status,
                ListDetails = servicePackage.ServiceDetails.Select(detail => new ServiceDetailReponse.ListDetailService
                {
                    ServiceDetailId = detail.ServiceDetailId,
                    Name = detail.Name,
                    Duration = detail.Duration,
                    Description = detail.Description,
                    PackageId = detail.PackageId,
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
                .Get(includeProperties: "ServiceDetails.PricePackages");
        //orderBy: q => q.OrderBy(), 

            if (servicePackages == null || !servicePackages.Any())
            {
                throw new ApiException(HttpStatusCode.NotFound, "No Service Packages found for landlords");
            }

            var responseList = servicePackages.Select(package => new ServicePackageLandlordResponse
            {
                PackageId = package.PackageId,
                Type = package.Type,
                HighLight = package.HighLight,
                Size = package.Size,
                Status = package.Status,
                ListServicePrice = package.ServiceDetails?.Select(serviceDetail => new ServicePriceResponse
                {
                    ServiceDetailId = serviceDetail.ServiceDetailId,
                    Name = serviceDetail.Name,
                    Duration = serviceDetail.Duration,
                    Description = serviceDetail.Description,
                    PackageId = serviceDetail.PackageId,
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


        public async Task CheckPackageRequest(string landlordId, string packageId)
        {
            var package = await _unitOfWork.ServicePackageRepository.GetPackageById(packageId);


            var currLContracts = await _landlordContractService.GetCurrentContracts(landlordId);

            if (package == null)
            {
                throw new Exception("Package not found");
            }
            else if (package.Status.Equals(StatusEnums.Inactive))
            {
                throw new Exception("Package inactive");
            }
            else if (!package.ServiceDetails.Any(sd => sd.PricePackages.Any(pk => pk.ApplicableDate <= DateTime.Now)))
            {
                throw new Exception("Package not applicable");
            }
            // Landlord used to buy package before
            else if (currLContracts.Any())
            {
                // CASE: currently using 1 package and have any other package
                if (currLContracts.Count() == 1)
                {
                    // get current package
                    var curContract = currLContracts.FirstOrDefault(c => c.StartDate <= DateTime.Now);

                    // extend current package
                    if (curContract!.PackageId == packageId)
                    {
                        if (curContract.EndDate >= DateTime.Now.AddDays(30))
                        {
                            throw new Exception("package payment not due");
                        }
                    }
                    // buy a new one
                    else
                    {
                        if (curContract != null && curContract.EndDate <= DateTime.Now)
                        {
                            throw new Exception("package payment not due");
                        }
                    }
                }else
                {
                    throw new Exception("");
                }
            }
        }

        public async Task<ServicePackage?> GetById(string packageId)
        {
            return await _unitOfWork.ServicePackageRepository.GetPackageById(packageId);
        }
    }
}
