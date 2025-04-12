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
using static QRCoder.PayloadGenerator.SwissQrCode;
using SP25_RPSC.Services.Service.NotificationService;
using SP25_RPSC.Services.Service.UserService;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Utils.Email;
using Microsoft.AspNetCore.SignalR;
using SP25_RPSC.Services.Service.Hubs.NotificationHub;
using Newtonsoft.Json;

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
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public PaymentService(ILandlordService LandlordService,
            IUnitOfWork unitOfWork,
            IPackageService packageService,
            ILandlordContractService landlordContractService,
            ICloudinaryStorageService cloudinaryStorageService,
            IPayOSService payOSService,
            ITransactionService transactionService,
            INotificationService notificationService,
            IUserService userService,
            IHubContext<NotificationHub> hubContext,
            IEmailService emailService)
        {
            _landlordService = LandlordService;
            _unitOfWork = unitOfWork;
            _packageService = packageService;
            _landlordContractService = landlordContractService;
            _httpClient = new HttpClient();
            _cloudinaryStorageService = cloudinaryStorageService;
            _payOSService = payOSService;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userService = userService;
            _emailService = emailService;
            _hubContext = hubContext;
        }

        public async Task<ResultModel> CreatePaymentPackageRequest(PaymentPackageRequestDTO paymentInfo, HttpContext context)
        {
            var Landlord = await _landlordService.GetLandlordById(paymentInfo.LandlordId);

            if (Landlord == null)
            {
                throw new Exception("Landlord not found.");
            }

            await _unitOfWork.LandlordContractRepository.RevokeExpirePackages(paymentInfo.LandlordId);

            await _packageService.CheckPackageRequest(paymentInfo.LandlordId, paymentInfo.PackageId);

            // unpaid trans
            var upTran = await _transactionService.GetUnpaidTransOfRepresentative(paymentInfo.LandlordId!);

            // check if unpaid tran exists
            if (upTran != null)
            {
                upTran.Status.Equals(StatusEnums.CANCELLED);
                await _transactionService.UpdateTransaction(upTran);

                // delete contract
                await _landlordContractService.DeleteContract(upTran.LcontractId);
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
                Price = (double)package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == paymentInfo.ServiceDetailId).PricePackages.FirstOrDefault(x => x.ApplicableDate <= DateTime.Now && x.Status.Equals(StatusEnums.Active.ToString())).Price,
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
                CancleUrl = "http://localhost:5173/landlord/confirmpayment",
                RedirectUrl = "http://localhost:5173/landlord/confirmpayment",
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

        public async Task<ResultModel> CreateExtendPackageRequest(PaymentExtendPackageRequestDTO extendInfo, HttpContext context)
        {
            // check Landlord
            var Landlord = await _landlordService.GetLandlordById(extendInfo.LandlordId);

            // CASE NOT EXISTED, throw 404 error - NOT FOUND
            if (Landlord == null)
            {
                throw new Exception("Landlord does not exist");
            }

            // revoke all old expired packages
            await _unitOfWork.LandlordContractRepository.RevokeExpirePackages(extendInfo.LandlordId);

            // check package
            await _packageService.CheckPackageRequest(extendInfo.LandlordId, extendInfo.PackageId);

            // check package
            var currContracts = await _landlordContractService.GetCurrentContracts(extendInfo.LandlordId);
            var currContract = currContracts.FirstOrDefault(u => u.PackageId == extendInfo.PackageId);

            if (currContract == null)
            {
                throw new Exception("Package payment overdue");
            }

            // package
            var package = await _packageService.GetById(extendInfo.PackageId);
            if (!package.ServiceDetails.Select(x => x.ServiceDetailId).ToList().Contains(extendInfo.ServiceDetailId)) throw new Exception("Service not found");

            // unpaid trans
            var upTran = await _transactionService.GetUnpaidTransOfRepresentative(extendInfo.LandlordId!);

            // check if unpaid tran exists
            if (upTran != null)
            {
                upTran.Status = StatusEnums.CANCELLED.ToString();
                await _transactionService.UpdateTransaction(upTran);
            }

            // init new tran
            var newTran = new Transaction()
            {
                TransactionId = Guid.NewGuid().ToString(),
                PaymentMethod = "Online",
                TransactionInfo = string.Empty,
                TransactionNumber = string.Empty,
                PaymentDate = DateTime.Now,
                Type = StatusEnums.EXTEND.ToString(),
                LcontractId = currContract.LcontractId,
                Amount = (double)package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == extendInfo.ServiceDetailId).PricePackages.FirstOrDefault(x => x.ApplicableDate <= DateTime.Now).Price,
                Status = StatusEnums.Processing.ToString(),
            };

            // add
            _transactionService.AddNewTransaction(newTran);

            // save
            await _unitOfWork.SaveAsync();

            object response = await _payOSService.CreatePaymentUrl(new PayOSReqModel
            {
                CancleUrl = "http://localhost:5173/landlord/confirmpayment",
                RedirectUrl = "http://localhost:5173/landlord/confirmpayment",
                PackageName = package!.ServiceDetails.FirstOrDefault(s => s.ServiceDetailId == extendInfo.ServiceDetailId).Name + package!.Type,
                Amount = (int)package!.ServiceDetails.FirstOrDefault(x => x.ServiceDetailId == extendInfo.ServiceDetailId).PricePackages.FirstOrDefault(x => x.ApplicableDate <= DateTime.Now).Price,
            });

            return new ResultModel
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Tạo thành công",
                Data = response
            };
        }

        public async Task<ResultModel> HandlePaymentPackageResponse(PaymentPackageResponseDTO response)
        {
            // check Landlord
            var Landlord = await _landlordService.GetLandlordById(response.LandlordId);

            // CASE NOT EXISTED, throw 404 error - NOT FOUND
            if (Landlord == null)
            {
                throw new Exception("Landlord does not exist");
            }

            // unpaid trans
            var upTran = await _transactionService.GetUnpaidTransOfRepresentative(response.LandlordId);

            if (upTran != null && upTran.Status.Equals(StatusEnums.Processing.ToString()))
            {
                string notifyDes = response.IsSuccess ? "Thanh toán thành công" : "Thanh toán thất bại";

                var notification = new Notification()
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    Message = notifyDes
                };

                await _notificationService.AddNotification(notification);

                // update noti to user
                Landlord.User.Notifications.Add(notification);
                await _userService.UpdateUser(Landlord.User);
                // send notification
                await _hubContext.Clients.Group(Landlord.User.UserId.ToString()).SendAsync("ReceiveNotification", JsonConvert.SerializeObject(notification));
                
                if (response.IsSuccess)
                {
                    // update trans
                    upTran.Status = StatusEnums.PAID.ToString();
                    upTran.TransactionInfo = response.TransactionInfo;
                    upTran.TransactionNumber = response.TransactionNumber;

                    // contract
                    if (upTran.Type.Equals(StatusEnums.EXTEND.ToString()))
                    {
                        // package
                        var package = await _packageService.GetById(upTran.Lcontract.PackageId);

                        var serviceDetail = package.ServiceDetails.FirstOrDefault();

                        var duration = serviceDetail?.Duration;
                        int parsedDuration = int.TryParse(duration, out var result) ? result : 0;

                        var pricePackage = serviceDetail?.PricePackages.FirstOrDefault();
                        var price = pricePackage?.Price ?? 0;
                        double priceAsDouble = (double)price;


                        // update contract url
                        // generate contract
                        var doc = await PdfGenerator.GenerateContractPdf(new LContractRequestDTO
                        {
                            CompanyName = Landlord.CompanyName,
                            LandlordAddress = Landlord.User.Address ?? "",
                            LandlordPhone = Landlord.User.PhoneNumber,
                            LandlordName = Landlord.User.FullName,
                            LandlordSignatureUrl = upTran.Lcontract.LandlordSignatureUrl,
                            PackageName = package!.Type,
                            StartDate = (DateTime)upTran.Lcontract.StartDate,
                            PaymentDate = (DateTime)upTran.Lcontract.Transactions.OrderBy(t => t.PaymentDate).FirstOrDefault()!.PaymentDate,
                            SignedDate = (DateTime)upTran.Lcontract.SignedDate,
                            Duration = upTran.Lcontract.Transactions.Count() * parsedDuration,
                            Price = upTran.Lcontract.Transactions.Count() * priceAsDouble,
                        }, _httpClient);

                        if (doc == null)
                        {
                            throw new Exception("Uploading contract occur some problems");
                        }

                        // upload contract image
                        var contractUrl = await _cloudinaryStorageService.UploadPdf(doc);

                        // update
                        upTran.Lcontract.LcontractUrl = contractUrl;
                        upTran.Lcontract.EndDate = upTran.Lcontract.EndDate.Value.AddDays(parsedDuration);
                    }
                    else
                    {
                        upTran.Lcontract.Status = StatusEnums.Active.ToString();
                    }
                }
                else
                {
                    // update trans
                    upTran.Status = StatusEnums.CANCELLED.ToString();

                    // cancel link
                    await _payOSService.CancelPaymentLink(long.Parse(response.TransactionNumber));

                    if (upTran.Type.Equals(StatusEnums.Buyed.ToString()))
                    {
                        // delete contract
                        await _landlordContractService.DeleteContract(upTran.LcontractId);
                    }
                }

                await _transactionService.UpdateTransaction(upTran);

                // save
                await _unitOfWork.SaveAsync();

                if (response.IsSuccess)
                {
                    string subject = "Bạn đã thanh toán thành công";
                    string html = EmailTemplate.EmailAfterPaymentTemplate(Landlord.User.FullName, upTran.Lcontract.LcontractUrl, subject);
                    // send email
                    _emailService.SendEmail(Landlord.User.Email, subject, html);
                }

                return new ResultModel
                {
                    Message = "Update Successfully",
                    Code = (int)HttpStatusCode.OK,
                };
            }

            return new ResultModel
            {
                Message = "Payment Fail",
                Code = (int)HttpStatusCode.BadRequest,
            };
        }
    }
}
