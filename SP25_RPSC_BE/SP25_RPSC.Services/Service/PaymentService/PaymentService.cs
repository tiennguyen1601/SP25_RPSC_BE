using Azure;
using CloudinaryDotNet.Actions;
using Org.BouncyCastle.Asn1.Ocsp;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Enums;
using SP25_RPSC.Data.Models.LContractModel.Request;
using SP25_RPSC.Data.Models.PackageModel.Request;
using SP25_RPSC.Data.Models.PayOSModel;
using SP25_RPSC.Data.Models.ResultModel;
using SP25_RPSC.Data.UnitOfWorks;
using SP25_RPSC.Services.Service.LandlordContractService;
using SP25_RPSC.Services.Service.LandlordService;
using SP25_RPSC.Services.Service.PackageService;
using SP25_RPSC.Services.Service.PayOSService;
using SP25_RPSC.Services.Service.TransactionService;
using SP25_RPSC.Services.Utils.PdfGenerator;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILandlordService _landlordService;
        private readonly ILandlordContractService _landlordContractService;
        private readonly IPackageService _packageService;
        private readonly ITransactionService _transactionService;
        private readonly HttpClient _httpClient;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly IPayOSService _payOSService;

        public PaymentService(ILandlordService LandlordService,
            IUnitOfWork unitOfWork,
            IPackageService packageService,
            ILandlordContractService landlordContractService,
            ICloudinaryStorageService cloudinaryStorageService,
            IPayOSService payOSService, ITransactionService transactionService)
        {
            _landlordService = LandlordService;
            _unitOfWork = unitOfWork;
            _packageService = packageService;
            _landlordContractService = landlordContractService;
            _httpClient = new HttpClient();
            _cloudinaryStorageService = cloudinaryStorageService;
            _payOSService = payOSService;
            _transactionService = transactionService;
        }

        public async Task<ResultModel> CreatePaymentPackageRequest(PaymentPackageRequestDTO paymentInfo, HttpContext context)
        {
            var Landlord = await _landlordService.GetLandlordById(paymentInfo.LandlordId);

            if (Landlord == null)
            {
                throw new Exception("Landlord not found.");
            }

            _unitOfWork.LandlordContractRepository.RevokeExpirePackages(paymentInfo.LandlordId);

            await _packageService.CheckPackageRequest(paymentInfo.LandlordId, paymentInfo.PackageId);

            // unpaid trans
            var upTran = await _transactionService.GetUnpaidTransOfRepresentative(paymentInfo.LandlordId!);

            // check if unpaid tran exists
            if (upTran != null)
            {
                upTran.Status.Equals(StatusEnums.Cancelled);
                _transactionService.UpdateTransaction(upTran);

                // delete contract
                _landlordContractService.DeleteContract(upTran.LcontractId);
            }

            // package
            var package = await _packageService.GetById(paymentInfo.PackageId);

            if (!package.ServiceDetails.Select(x => x.ServiceDetailId).ToList().Contains(paymentInfo.ServiceDetailId)) throw new Exception("Service not found");

            // check package
            var currContracts = await _landlordContractService.GetCurrentContracts(paymentInfo.LandlordId);
            var currContract = currContracts.FirstOrDefault(u => u.StartDate <= DateTime.Now);

            // upload customer signature
            var cusSignUrl = await _cloudinaryStorageService.UploadImage(paymentInfo.SignatureFile);

            var signedDate = DateTime.Now;

            var newContract = new LContractRequestDTO
            {
                CompanyName = Landlord.CompanyName,
                LandlordAddress = Landlord.User.Address ?? "",
                LandlordPhone = Landlord.User.PhoneNumber,
                LandlordName = Landlord.User.FullName,
                LandlordSignatureUrl = cusSignUrl,
                PackageName = package!.Type,
                ServiceName = package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId).Name,
                StartDate = currContract != null ? currContract.EndDate.HasValue ? currContract.EndDate.Value.AddDays(1) : DateTime.Now : DateTime.Now,
                PaymentDate = DateTime.Now,
                SignedDate = signedDate,
                Duration = int.Parse(package!.ServiceDetails
            .FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId)?.Duration ?? "0"),
                Price = (double)package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId).PricePackages.FirstOrDefault(x => x.ApplicableDate <= DateTime.Now).Price,
            };

            // generate contract
            var doc = await PdfGenerator.GenerateContractPdf(newContract, _httpClient);

            if (doc == null)
            {
                throw new Exception("Uploading contract occur some problems");
            }

            // upload contract image
            var contractUrl = await _cloudinaryStorageService.UploadPdf(doc);

            // init new uni package
            var lContract = new LandlordContract
            {
                LcontractId = Guid.NewGuid().ToString(),
                Status = StatusEnums.Inactive.ToString(),
                LandlordId = paymentInfo.LandlordId,
                PackageId = package!.PackageId,
                StartDate = currContract != null ? currContract.EndDate.HasValue ? currContract.EndDate.Value.AddDays(1) : DateTime.Now : DateTime.Now,
                EndDate = currContract != null ? currContract.EndDate.HasValue ? currContract.EndDate.Value.AddMonths(currContract.Transactions.Count() * int.Parse(package.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId)?.Duration ?? "0")) : DateTime.Now : DateTime.Now.AddDays(double.Parse(package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId)?.Duration ?? "0")),
                LcontractUrl = contractUrl,
                SignedDate = signedDate,
                LandlordSignatureUrl = cusSignUrl
            };

            // add unipack
            await _landlordContractService.InsertContract(lContract);


            // init new tran
            var newTran = new Transaction()
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentMethod = "Online",
                TransactionInfo = string.Empty,
                TransactionNumber = string.Empty,
                PaymentDate = DateTime.Now,
                LcontractId = lContract.LcontractId,
                Type = StatusEnums.Buyed.ToString(),
                Amount = (double)package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId).PricePackages.FirstOrDefault(x => x.ApplicableDate <= DateTime.Now).Price,
                Status = StatusEnums.Processing.ToString(),
            };

            // add
             await _transactionService.AddNewTransaction(newTran);

            // save
            await _unitOfWork.SaveAsync();

            object response = await _payOSService.CreatePaymentUrl(new PayOSReqModel
            {
                CancleUrl = "https://localhost:7159/swagger/index.html",
                RedirectUrl = "https://localhost:7159/swagger/index.html",
                PackageName = package!.ServiceDetails.FirstOrDefault(s => s.ServiceDetailId == paymentInfo.ServiceDetailId).Name + package!.Type,
                Amount = (int)package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId).PricePackages.FirstOrDefault(x => x.ApplicableDate <= DateTime.Now).Price,
            });

            return new ResultModel
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Tạo thành công",
                Data = response
            };
        }
    }
}
