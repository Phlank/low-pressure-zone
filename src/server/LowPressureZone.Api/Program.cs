using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json")
                     .AddJsonFile("appsettings.Development.json", optional: true)
                     .AddJsonFile("appsettings.Production.json", optional: true);

builder.AddDatabases();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration.GetValue<string>("JwtSigningKey"));
builder.Services.AddAuthorization();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.UseProblemDetails();
}).UseSwaggerGen();

app.UseHsts();
app.Run();
