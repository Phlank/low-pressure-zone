using FastEndpoints;
using FastEndpoints.Swagger;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json")
                     .AddJsonFile("appsettings.Development.json", optional: true)
                     .AddJsonFile("appsettings.Production.json", optional: true);

builder.Services.AddFastEndpoints()
                .SwaggerDocument();

builder.AddDatabases();
builder.Services.AddIdentity<IdentityUser, IdentityRole>();

var app = builder.Build();

app.RunDatabaseMigrations();

app.UseFastEndpoints(config =>
{
    config.Errors.UseProblemDetails();
}).UseSwaggerGen();

app.UseHsts();
app.Run();
