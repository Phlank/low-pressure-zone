using FastEndpoints;
using FastEndpoints.Swagger;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints()
                .SwaggerDocument();

builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseSqlite(sqliteOptions =>
    {
        sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        sqliteOptions.MigrationsAssembly("LowPressureZone.Identity");
    });
});

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlite(sqliteOptions =>
    {
        sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        sqliteOptions.MigrationsAssembly("LowPressureZone.Domain");
    });
});

var app = builder.Build();

app.UseFastEndpoints((config) =>
{
    config.Errors.UseProblemDetails();
}).UseSwaggerGen();

app.UseHsts();
app.Run();
