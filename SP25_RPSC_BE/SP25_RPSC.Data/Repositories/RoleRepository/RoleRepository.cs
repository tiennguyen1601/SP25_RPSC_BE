using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.RoleRepository
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
       Role GetRoleByRoleName(string roleName);
    }

    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly RpscContext _context;

        public RoleRepository(RpscContext context) : base(context)
        {
            _context = context;
        }

        public Role GetRoleByRoleName(string roleName)
        {
           return (Role)_context.Roles.Where(x => x.RoleName == roleName);
        }
    }
}
