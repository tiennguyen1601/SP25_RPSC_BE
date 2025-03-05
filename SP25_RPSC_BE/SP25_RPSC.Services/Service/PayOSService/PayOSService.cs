using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using SP25_RPSC.Data.Models.PayOSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PayOSService
{
    public class PayOSService :IPayOSService
    {
        private readonly IConfiguration _config;
        private readonly PayOS _payOS;

        public PayOSService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<CreatePaymentResult> CreatePaymentUrl(PayOSReqModel payOSReqModel)
        {
            PayOS payOS = new PayOS(
                _config["PayOS:ClientID"],
                _config["PayOS:ApiKey"],
                _config["PayOS:ChecksumKey"]
            );

            // Tạo item thanh toán
            ItemData item = new ItemData(payOSReqModel.PackageName, 1, (int)payOSReqModel.Amount);
            List<ItemData> items = new List<ItemData> { item };

            // Chuẩn bị dữ liệu thanh toán
            PaymentData paymentData = new PaymentData(
                payOSReqModel.OrderId,
                (int)payOSReqModel.Amount,
                payOSReqModel.PackageName,
                items,
                payOSReqModel.CancleUrl,
                payOSReqModel.RedirectUrl
            );

            CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

            return createPayment;
        }

        public async Task<PaymentLinkInformation> CancelPaymentLink(long orderCode)
        {
            PaymentLinkInformation cancelledPaymentLinkInfo = await _payOS.cancelPaymentLink(orderCode);

            return cancelledPaymentLinkInfo;
        }
    }
}
