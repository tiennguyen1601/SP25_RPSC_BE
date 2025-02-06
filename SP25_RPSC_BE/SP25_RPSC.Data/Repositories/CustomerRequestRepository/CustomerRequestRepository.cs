﻿using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.CustomerRequestRepository
{
    public interface ICustomerRequestRepository : IGenericRepository<CustomerRequest>
    {

    }

    public class CustomerRequestRepository : GenericRepository<CustomerRequest>, ICustomerRequestRepository
    {
        private readonly RpscContext _context;

        public CustomerRequestRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
