using FastEndpoints;
using FastEndpoints.Swagger;
using Google.Apis.Auth.AspNetCore3;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

builder.Services.AddOptions<OpenIdConnectOptions>("Authentication:Google");
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    authOptions.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    authOptions.DefaultScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
}).AddJwtBearer().AddGoogleOpenIdConnect(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId");
    options.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret");
    options.Events = new OpenIdConnectEvents()
    {
        OnTokenValidated = async (context) =>
        {
            var name = context.Principal?.Identity?.Name;
        },
        OnUserInformationReceived = async (context) =>
        {
            var name = context.Principal?.Identity?.Name;
            if (name == null)
            {
                await context.Response.SendForbiddenAsync();
                return;
            }
            // Create user if it doesn't exist in the identity db
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();

        }
    };
});
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
