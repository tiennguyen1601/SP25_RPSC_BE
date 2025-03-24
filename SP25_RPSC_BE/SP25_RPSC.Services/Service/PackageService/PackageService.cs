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

            var package = new ServicePackage
            {
                Type = model.Type,
                HighLightTime = model.HighLightTime,
                MaxPost = model.MaxPost,
                Label = model.Label,
                Status = StatusEnums.Active.ToString(),
            };

            await _unitOfWork.ServicePackageRepository.Add(package);
        }


        public async Task CreateServiceDetail(ServiceDetailCreateRequestModel model)
        {
            if (model == null)
            {
                throw new ArgumentException("Dữ liệu không hợp lệ.");
            }

            if (!decimal.TryParse(model.Price.ToString(), out decimal validPrice) || validPrice <= 0)
            {
                throw new ArgumentException("Price phải là số hợp lệ lớn hơn 0.");
            }
            if (!int.TryParse(model.Duration, out int validDuration) || validDuration <= 0)
            {
                throw new ArgumentException("Duration phải là số nguyên lớn hơn 0.");
            }

            var package = await _unitOfWork.ServicePackageRepository.GetByIDAsync(model.PackageId);
            if (package == null)
            {
                throw new Exception("Package không tồn tại.");
            }

            var serviceDetail = new ServiceDetail
            {
                ServiceDetailId = Guid.NewGuid().ToString(),
                Name = model.Name,
                Duration = validDuration.ToString(),
                Description = model.Description,
                Status = StatusEnums.Active.ToString(),
                PackageId = model.PackageId
            };

            var pricePackage = new PricePackage
            {
                PriceId = Guid.NewGuid().ToString(),
                Price = validPrice,
                ApplicableDate = DateTime.UtcNow,
                Status = StatusEnums.Active.ToString(),
                ServiceDetailId = serviceDetail.ServiceDetailId
            };

            serviceDetail.PricePackages.Add(pricePackage);

            await _unitOfWork.ServiceDetailRepository.Add(serviceDetail);
            await _unitOfWork.SaveAsync();
        }





        public async Task<List<ServicePackageReponse>> GetAllServicePackage()
        {
            var servicePackages = await _unitOfWork.ServicePackageRepository.Get(
                orderBy: c => c.OrderBy(package => package.Type)
                );

            if (servicePackages == null || !servicePackages.Any())
            {
                throw new ApiException(HttpStatusCode.BadRequest, "ServicePackageEmpty");
            }

            return _mapper.Map<List<ServicePackageReponse>>(servicePackages);
        }



        public async Task<ServiceDetailReponse?> GetServiceDetailsByPackageId(string packageId)
        {
            var servicePackage = (await _unitOfWork.ServicePackageRepository
                .Get(sp => sp.PackageId == packageId, includeProperties: "ServiceDetails.PricePackages"
                , orderBy: c => c.OrderBy(package => package.Type)
                )).FirstOrDefault();

            if (servicePackage == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "No ServiceDetails found for the given PackageId");
            }
            var response = new ServiceDetailReponse
            {
                PackageId = servicePackage.PackageId,
                Type = servicePackage.Type,
                HighLightTime = servicePackage.HighLightTime,
                MaxPost = servicePackage.MaxPost,
                label = servicePackage.Label,
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
                .OrderBy(x => x.Duration)
                .OrderBy(x => x.Price)
                .ToList()
            };

            return response;
        }


        public async Task<List<ServicePackageLandlordResponse>> GetServicePackageForLanlord()
        {
            var servicePackages = await _unitOfWork.ServicePackageRepository
                .Get(includeProperties: "ServiceDetails.PricePackages"
                , orderBy: c => c.OrderBy(package => package.Type));
        //orderBy: q => q.OrderBy(), 

            if (servicePackages == null || !servicePackages.Any())
            {
                throw new ApiException(HttpStatusCode.NotFound, "No Service Packages found for landlords");
            }

            var responseList = servicePackages.Select(package => new ServicePackageLandlordResponse
            {
                PackageId = package.PackageId,
                Type = package.Type,
                HighLightTime = package.HighLightTime,
                MaxPost = package.MaxPost,
                Label = package.Label,
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


        public async Task UpdatePriceAndServiceDetail(string priceId, decimal newPrice, string newName, string newDuration, string newDescription)
        {
            var pricePackage = await _unitOfWork.PricePackageRepository.GetByIDAsync(priceId);

            if (pricePackage == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "No price package found for the given PriceId.");
            }

            var serviceDetail = await _unitOfWork.ServiceDetailRepository.GetByIDAsync(pricePackage.ServiceDetailId);

            if (serviceDetail == null)
            {
                throw new ApiException(HttpStatusCode.NotFound, "No ServiceDetail found for the given PriceId.");
            }

            serviceDetail.Name = newName;
            serviceDetail.Duration = newDuration;
            serviceDetail.Description = newDescription;
            await _unitOfWork.ServiceDetailRepository.Update(serviceDetail);

            pricePackage.Status = StatusEnums.Inactive.ToString();
            await _unitOfWork.PricePackageRepository.Update(pricePackage);

            var newPricePackage = new PricePackage
            {
                PriceId = Guid.NewGuid().ToString(),
                Price = newPrice,
                Status = StatusEnums.Active.ToString(),
                ApplicableDate = DateTime.UtcNow,
                ServiceDetailId = serviceDetail.ServiceDetailId
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
