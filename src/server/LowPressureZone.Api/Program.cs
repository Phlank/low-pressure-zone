using FastEndpoints;
using FastEndpoints.Swagger;
using LowPressureZone.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json")
                     .AddJsonFile("appsettings.Development.json", optional: true)
                     .AddJsonFile("appsettings.Production.json", optional: true);

builder.Services.AddFastEndpoints()
                .SwaggerDocument();

builder.AddDatabases();

var app = builder.Build();

app.UseFastEndpoints((config) =>
{
    config.Errors.UseProblemDetails();
}).UseSwaggerGen();

app.UseHsts();
app.Run();
