using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Enums
{
    public class ReasonEnums
    {
        public const string KickRoommateReason = "Có vẻ bạn đã có hành vi không phù hợp trong việc tuân thủ luật ở ghép.";
        public const string KickTenantReason = "Có vẻ bạn đã có hành vi không phù hợp trong việc tuân thủ luật thuê phòng trọ.";
        public const string RejectSharingReason = "Có vẻ như 2 bạn hơi khác biệt về phong cách sống.";
        public const string InactivateRoommatePostReason = "Có vẻ như bài đăng của bạn đã vi phạm một số tiêu chuẩn cộng đồng.";
        public const string InactivateRoommatePostReasonForRequest = "Có vẻ như bài đăng đã vi phạm một số tiêu chuẩn cộng đồng.";
        public const string InactivateCustomerAccount = "Có vẻ như bạn đã vi phạm một số tiêu chuẩn cộng đồng.";
    }
}
