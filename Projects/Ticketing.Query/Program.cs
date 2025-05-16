using Ticketing.Query.Application;
using Ticketing.Query.Application.Extensions;
using Ticketing.Query.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.RegisterInfrastructureServices(builder.Configuration);
builder.Services.RegisterApplicationServices();

var app = builder.Build();

app.MapControllers();

//Apply migrations 
await app.ApplyMigration();

app.Run();
