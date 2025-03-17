using Microsoft.AspNetCore.Http;
using SP25_RPSC.Data.Models.PackageModel.Request;
using SP25_RPSC.Data.Models.PayOSModel;
using SP25_RPSC.Data.Models.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PaymentService
{
    public interface IPaymentService
    {
        Task<ResultModel> CreatePaymentPackageRequest(PaymentPackageRequestDTO paymentInfo, HttpContext context);
        Task<ResultModel> HandlePaymentPackageResponse(PaymentPackageResponseDTO response);
    }
}
