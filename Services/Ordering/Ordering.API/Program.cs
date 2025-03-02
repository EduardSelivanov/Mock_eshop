using CommonPractices.Carter;
using CommonPractices.Exceptions;
using Ordering.Application.Exts;
using Ordering.Presentation.Extensions;


namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddExceptionHandler<CommonExceptionHandler>();

            builder.Services.AddAppExts(builder.Configuration);
            builder.Services.AddPresentation();

            var app = builder.Build();
            app.UseCarterConfig();
            app.UseExceptionHandler(options => { });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

           


       

            app.Run();
        }
    }
}
