using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SlotsService.Data;
using SlotsService.Repos;
using SlotsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<SlotsContext>
    (opts=>opts.UseSqlServer(builder.Configuration.GetConnectionString("SlotsDataBase")));

builder.Services.AddScoped<ISlotRepo, SlotRepo>();
builder.Services.Decorate<ISlotRepo, CachedSlotRepo>();

//builder.Services.AddScoped<ISlotRepo>(provider =>
//{
//    var slotRepo = provider.GetRequiredService<SlotRepo>();
//    return new CachedSlotRepo(slotRepo, provider.GetRequiredService<IDistributedCache>());
//});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "127.0.0.1:6379";
});


var app = builder.Build();
app.MapGrpcService<SlotService>();

// Configure the HTTP request pipeline.


app.Run();
