using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WareHouse.Domain.Reopositories;
using WareHouse.Infrastructure.DataBase;
using WareHouse.Infrastructure.Repos;

namespace WareHouse.Infrastructure.Extensions
{
    public static class InfrExts
    {
        public static void AddInfrastructure(this IServiceCollection services,IConfiguration config)
        {
            string connstr = config.GetConnectionString("WareHouseDb");

            services.AddDbContext<WareHouseContext>(opt=>opt.UseSqlServer(connstr));
            services.AddScoped<IRackRepo, RackRepo>();

        }

    }
}
