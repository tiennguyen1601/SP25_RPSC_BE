using Microsoft.EntityFrameworkCore;
using SP25_RPSC.Data.Repositories;
using SP25_RPSC.Data.UnitOfWorks;

namespace SP25_RPSC.Controllers.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<RpscContext>(options => options.UseSqlServer(GetConnectionString()));
            return services;
        }
        private static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
            var strConn = config["ConnectionStrings:DefaultConnectionString"];

            return strConn;
        }
    }
}
