using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LowPressureZone.Identity;

public class IdentityContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<Invitation<Guid, AppUser>> Invitations { get; set; }

    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            var roles = context.Set<AppRole>().ToList();
            foreach (var role in RoleNames.All)
            {
                if (!roles.Any(r => r.Name == role))
                {
                    context.Set<AppRole>().Add(new AppRole
                    {
                        Id = Guid.NewGuid(),
                        Name = role,
                        NormalizedName = role.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }
            }

            var adminRole = context.Set<IdentityRole>().First(r => r.Name == RoleNames.Admin);
            if (!context.Set<AppUser>().Any())
            {
                var seedData = GetSeedData();
                var hasher = new PasswordHasher<AppUser>();
                var user = new AppUser()
                {
                    Id = Guid.NewGuid(),
                    AccessFailedCount = 0,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Email = seedData.AdminEmail,
                    NormalizedEmail = seedData.AdminEmail.ToUpper(),
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEnd = null,
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = true,
                    UserName = seedData.AdminUsername,
                    NormalizedUserName = seedData.AdminUsername.ToUpper(),
                };
                var passwordHash = hasher.HashPassword(user, seedData.AdminPassword);
                user.PasswordHash = passwordHash;
                context.Set<AppUser>().Add(user);
                
            }
            context.SaveChanges();
        });
    }

    private IdentitySeedData GetSeedData()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                              .AddJsonFile("appsettings.Development.json", optional: true)
                                                              .AddJsonFile("appsettings.Production.json", optional: true)
                                                              .Build();
        var section = config.GetRequiredSection("SeedData:Identity") ?? throw new Exception("No admin email specified in configuration.");
        return section.Get<IdentitySeedData>() ?? throw new NullReferenceException("Seed data missing from configuration.");
    }
}
