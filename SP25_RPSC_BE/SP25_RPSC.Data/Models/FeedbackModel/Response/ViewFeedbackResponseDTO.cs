using SP25_RPSC.Data.Models.LContractModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.FeedbackModel.Response
{
    public class ViewFeedbackResponseDTO
    {
        public int TotalFeedback { get; set; }
        public List<ListFeedbackRoomeRes> Feebacks { get; set; }

    }

    public class ListFeedbackRoomeRes
    {
        public string FeedbackID { get; set; }

        public string ReviewerName { get; set; }

        public string ReviewerPhoneNumber { get; set; }

        public string? Type { get; set; }

        public string? RoomNumber { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
