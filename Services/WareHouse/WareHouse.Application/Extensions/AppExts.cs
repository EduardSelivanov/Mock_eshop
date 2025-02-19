using CommonPractices.CQRS.Behaviours;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using SlotsService;



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

            services.AddGrpcClient<SlotsProtoService.SlotsProtoServiceClient>(client =>
            {
                client.Address = new Uri("https://localhost:7298");
            });

            services.AddSingleton<IMapper, Mapper>(); 
        }
    }
}
