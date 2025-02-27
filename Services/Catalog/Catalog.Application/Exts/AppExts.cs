
using Catalog.Application.MassTransit;
using Elastic.CommonSchema;
using Marten;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SlotsService;

namespace Catalog.Application.Exts
{
    public static class AppExts
    {
        public static void AppExsts(this IServiceCollection services, IConfiguration config)
        {
            var assembly = typeof(AppExts).Assembly;

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(assembly);
                //config.AddOpenBehavior(typeof)
            });

            services.AddMarten(opts =>
            {
                opts.Connection(config.GetConnectionString("CatalogDatabase"));
            }).UseLightweightSessions();

            services.AddGrpcClient<SlotsProtoService.SlotsProtoServiceClient>(client =>
            {
                client.Address = new Uri("https://localhost:7298");
            });

            services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("127.0.0.1", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("order_created_queue", e =>
                    {
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    });
                });
            });
        }
    }
}
