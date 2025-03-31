using SP25_RPSC.Data.Models.AmentitiesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.AmentyService
{
    public interface IAmentyService
    {
        Task<bool> CreateAmenty (RoomAmentyRequestCreateModel model, string token);
        Task<GetAllAmentiesResponseModel> GetAllAmentiesByLandlordId(string searchQuery, int pageIndex, int pageSize, string token);
    }
}
