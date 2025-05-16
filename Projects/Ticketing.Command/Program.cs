using Scalar.AspNetCore;
using Ticketing.Command.Application;
using Ticketing.Command.Features.Apis;
using Ticketing.Command.Infrastructure;
//using static Ticketing.Command.Features.Tickets.TicketCreate;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.RegisterMinimalApis();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Microservice Command with Scalar";
        opt.DarkMode = true;
        opt.Theme = ScalarTheme.Purple;
        opt.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Http, ScalarClient.Http11);
    });
}

//
//app.MapPost("/api/ticket", async (IMediator mediator, TicketCreateRequest request, CancellationToken cancellationToken) =>
//{
//    var command = new TicketCreateCommand(request);
//    var result = await mediator.Send(command, cancellationToken);
//    return Results.Ok(result);
//}).WithName("CreateTicket");

app.MapMinimalApisEndpoints();

app.Run();
