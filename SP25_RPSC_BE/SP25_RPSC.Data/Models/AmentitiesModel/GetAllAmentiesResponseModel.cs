using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Models.AmentitiesModel
{
    public class GetAllAmentiesResponseModel
    {
        public List<ListAmentyRes> Amenties { get; set; }
        public int TotalAmenties { get; set; }
    }
    public class ListAmentyRes
    {
        public string RoomAmentyId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        public decimal? Compensation { get; set; }
    }

}
