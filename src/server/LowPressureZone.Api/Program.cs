using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using Humanizer;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minerals.StringCases;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json");
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
}
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.Production.json", optional: true);
}

builder.AddDatabases();
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.AddMappers();
builder.AddServices();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "LowPressureZoneCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
});
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("Development", builder =>
        {
            builder.WithOrigins("http://localhost:4001")
                   .AllowAnyHeader()
                   .WithMethods("GET", "PUT", "POST", "DELETE");
        });
    }
    else
    {
        options.AddPolicy("Production", builder =>
        {
            builder.WithOrigins("https://lowpressurezone.com")
                   .AllowAnyHeader()
                   .WithMethods("GET", "PUT", "POST", "DELETE");
        });
    }
});

var app = builder.Build();
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new ValidationProblemDetails(
            failures.GroupBy(f => (f.PropertyName ?? "none").ToCamelCase())
                    .ToDictionary(
                        keySelector: e => e.Key,
                        elementSelector: e => e.Select(m => m.ErrorMessage).ToArray()))
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = statusCode,
            Instance = ctx.Request.Path,
            Extensions = { { "traceId", ctx.TraceIdentifier } }
        };
    };
    config.Errors.ProducesMetadataType = typeof(ValidationProblemDetails);
}).UseSwaggerGen();
app.UseHsts();
app.Run();
