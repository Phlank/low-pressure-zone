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
            var roles = context.Set<AppRole>();
            foreach (var roleName in RoleNames.All)
            {
                Console.WriteLine($"Checking for {roleName} role...");
                if (!roles.Any(r => r.Name == roleName))
                {
                    Console.WriteLine($"Did not find {roleName} role. Adding {roleName} role...");
                    roles.Add(new AppRole
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }
            }
            context.SaveChanges();

            Console.WriteLine("Retrieving admin role...");
            var adminRole = roles.First(r => r.Name == RoleNames.Admin);
            var users = context.Set<AppUser>();
            if (!users.Any())
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
                users.Add(user);

                var userRoles = context.Set<IdentityUserRole<Guid>>();
                userRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = adminRole.Id });
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
