using CommonPractices.RabbitMQContracts;
using Marten;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Services;

namespace Ordering.Application.Exts
{
    public static class AppExts
    {
        public static void AddAppExts(this IServiceCollection services,IConfiguration config)
        {
            services.AddMarten(opts =>
            {
                opts.Connection(config.GetConnectionString("OrderDatabase"));
            }).UseLightweightSessions();

            services.AddMassTransit(x =>
            {

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("127.0.0.1", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    
                });
                x.AddRequestClient<CreateOrderAMQ>();
            });

            
            
            services.AddScoped<RabbitService>();
            services.AddScoped<PaymentService>();
        }
    }
}
