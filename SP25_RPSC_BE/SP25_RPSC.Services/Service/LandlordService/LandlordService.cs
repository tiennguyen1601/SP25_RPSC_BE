using CloudinaryDotNet.Actions;
using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Services.Service.LandlordService
{
    public class LandlordService : ILandlordService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LandlordService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Landlord?> GetLandlordById(string id)
        {
            return await _unitOfWork.LandlordRepository.GetAsync(id);
        }

    }
}
