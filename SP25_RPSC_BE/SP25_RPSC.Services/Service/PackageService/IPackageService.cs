using SP25_RPSC.Data.Models.PackageModel;
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
    }
}
