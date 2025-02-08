﻿using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.OTPRepository
{
    public interface IOTPRepository : IGenericRepository<Otp>
    {

    }

    public class OTPRepository : GenericRepository<Otp>, IOTPRepository
    {
        private readonly RpscContext _context;

        public OTPRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
