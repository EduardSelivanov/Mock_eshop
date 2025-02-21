using Catalog.Application.Exts;
using Catalog.Presentation.Exts;
using CommonPractices.Carter;
using CommonPractices.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AppExsts(builder.Configuration);
builder.Services.AddPrsnt();
builder.Services.AddExceptionHandler<CommonExceptionHandler>();


var app = builder.Build();
app.UseCarterConfig();
app.UseExceptionHandler(options => { });




app.Run();
