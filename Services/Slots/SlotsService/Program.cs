using Microsoft.EntityFrameworkCore;
using SlotsService.Data;
using SlotsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<SlotsContext>
    (opts=>opts.UseSqlServer(builder.Configuration.GetConnectionString("SlotsDataBase")));

var app = builder.Build();
app.MapGrpcService<SlotService>();

// Configure the HTTP request pipeline.


app.Run();
