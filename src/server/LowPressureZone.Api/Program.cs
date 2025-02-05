using FastEndpoints;
using FastEndpoints.Swagger;
using Google.Apis.Auth.AspNetCore3;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>();

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
    options.Scope.Add("openid");
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
    options.Events = new OpenIdConnectEvents()
    {
        OnTokenValidated = async (context) =>
        {
            var emailClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type.Contains("email"));
            if (emailClaim == null)
            {
                return;
            }
            Console.WriteLine($"{emailClaim.Type}: {emailClaim.Value}");
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(emailClaim.Value);
            if (user == null)
            {
                throw new Exception("Individual has no corresponding user in the database.");
            }
        },
        OnTicketReceived = (context) =>
        {
            context.ReturnUri = builder.Configuration.GetValue<string>("Client:BaseUrl") + builder.Configuration.GetValue<string>("Client:LoginRedirect");
            return Task.CompletedTask;
        },
    };
});
builder.Services.AddAuthorization();
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
                   .AllowAnyMethod();
        });
    } 
    else
    {
        options.AddPolicy("Production", builder =>
        {
            builder.WithOrigins("https://lowpressurezone.com")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
    }
});

var app = builder.Build();
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");
app.UseRedirectUnauthorizedToChallengeEndpoint();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
    {
        return new ValidationProblemDetails(
            failures.GroupBy(f => f.PropertyName)
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
