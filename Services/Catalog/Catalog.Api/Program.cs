using Carter;
using Catalog.Application.Exts;
using Catalog.Presentation.Exts;
using CommonPractices.Carter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AppExsts(builder.Configuration);
builder.Services.AddPrsnt();


var app = builder.Build();
app.UseCarterConfig();




app.Run();
