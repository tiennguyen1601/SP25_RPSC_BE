using SP25_RPSC.Data.Entities;
using SP25_RPSC.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP25_RPSC.Data.Repositories.FavoriteRepository
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {

    }

    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        private readonly RpscContext _context;

        public FavoriteRepository(RpscContext context) : base(context)
        {
            _context = context;
        }
    }
}
