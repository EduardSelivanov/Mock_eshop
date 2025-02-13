using CommonPractices.CQRS.Behaviours;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;



namespace WareHouse.Application.Extensions
{
    public static class AppExts
    {
        public static void AddAppExts(this IServiceCollection services)
        {
            var assembly = typeof(AppExts).Assembly;
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(LoggingBeh<,>));
            });

            services.AddSingleton<IMapper, Mapper>(); 
        }
    }
}
