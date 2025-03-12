using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Models.PackageModel;
using SP25_RPSC.Data.Models.PackageServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PackageService
{
    public interface IPackageService
    {
        Task CreatePackage(PackageCreateRequestModel model);

        Task<List<ServicePackageReponse>> GetAllServicePackage();

        Task<ServiceDetailReponse?> GetServiceDetailsByPackageId(string packageId);
        Task<List<ServicePackageLandlordResponse>> GetServicePackageForLanlord();
        Task CheckPackageRequest(string landlordId, string packageId);
        Task<ServicePackage?> GetById(string packageId);
    }
}
