    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace SP25_RPSC.Data.Models.RoomModel.RequestModel
    {
        public class PostRoomCreateRequestModel
        {
            public string RoomId { get; set; } = null!;
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public int DateExist { get; set; }  
            public DateTime? AvailableDateToRent { get; set; }
        }
    }
