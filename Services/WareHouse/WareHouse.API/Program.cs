using Carter;
using CommonPractices.Carter;
using CommonPractices.Exceptions;
using CommonPractices.SerilogSett;
using Scalar.AspNetCore;
using Serilog;
using WareHouse.Application.Extensions;
using WareHouse.Infrastructure.Extensions;
using WareHouse.Presentation.Exts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
builder.Services.AddPresentation();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppExts();
builder.Services.AddExceptionHandler<CommonExceptionHandler>();



SerilogSettings.configureLogging(builder.Configuration["ElasticConfig:Uri"]);
builder.Host.UseSerilog();



var app = builder.Build();
app.UseCarterConfig();
app.UseExceptionHandler(options => { });



app.MapScalarApiReference();




app.MapGet("/", () => "Hello World!");



app.Run();

// \Services\WareHouse