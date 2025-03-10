using Net.payOS.Types;
using SP25_RPSC.Data.Models.PayOSModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.PayOSService
{
    public interface IPayOSService
    {
        Task<CreatePaymentResult> CreatePaymentUrl(PayOSReqModel payOSReqModel);
        Task<PaymentLinkInformation> CancelPaymentLink(long orderCode);
    }
}
