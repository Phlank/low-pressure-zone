using LowPressureZone.Identity.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LowPressureZone.Identity;

public class IdentityContext : IdentityDbContext<IdentityUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var adminEmail = GetSeedAdminEmail();

        optionsBuilder.UseSeeding((context, _) =>
        {
            var roles = context.Set<IdentityRole>().ToList();
            foreach (var role in Constants.RoleNames.AllRoles)
            {
                if (!roles.Any(r => r.Name == role))
                {
                    context.Set<IdentityRole>().Add(new IdentityRole(role));
                }
            }

            var adminRole = context.Set<IdentityRole>().First(r => r.Name == Constants.RoleNames.Admin);
            if (!context.Set<IdentityUser>().Any())
            {
                var user = new IdentityUser("AdminUser")
                {
                    Email = adminEmail,
                };
                context.Set<IdentityUser>().Add(user);
                context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = user.Id
                });
            }
            context.SaveChanges();
        });
    }

    private string GetSeedAdminEmail()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                        .AddJsonFile("appsettings.Development.json", optional: true)
                                                                        .AddJsonFile("appsettings.Production.json", optional: true)
                                                                        .Build();
        return config.GetValue<string>("SeedData:AdminEmail") ?? throw new Exception("No admin email specified in configuration.");
    }
}
