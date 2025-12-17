using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LowPressureZone.Identity;

public class IdentityContext(DbContextOptions<IdentityContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options), IDataProtectionKeyContext
{
    public DbSet<Invitation<Guid, AppUser>> Invitations { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSeeding((context, _) =>
        {
            Console.WriteLine($"{nameof(IdentityContext)}: Seeding db.");
            var roles = context.Set<AppRole>();
            foreach (var roleName in RoleNames.DatabaseRoles)
            {
                Console.WriteLine($"{nameof(IdentityContext)}: Checking for {roleName} role.");
                if (roles.Any(r => r.Name == roleName)) continue;
                Console.WriteLine($"{nameof(IdentityContext)}: Did not find {roleName} role. Adding {roleName} role.");
                roles.Add(new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }

            context.SaveChanges();

            var users = context.Set<AppUser>();
            if (!users.Any())
            {
                Console.WriteLine($"{nameof(IdentityContext)}: No users found. Seeding admin user.");
                var seedData = GetSeedData();
                var hasher = new PasswordHasher<AppUser>();
                var user = new AppUser
                {
                    Id = Guid.NewGuid(),
                    AccessFailedCount = 0,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Email = seedData.AdminEmail,
                    NormalizedEmail = seedData.AdminEmail.ToUpperInvariant().Normalize(),
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    LockoutEnd = null,
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    TwoFactorEnabled = true,
                    DisplayName = seedData.AdminDisplayName,
                    UserName = seedData.AdminUsername,
                    NormalizedUserName = seedData.AdminUsername.ToUpperInvariant().Normalize()
                };
                var passwordHash = hasher.HashPassword(user, seedData.AdminPassword);
                user.PasswordHash = passwordHash;
                users.Add(user);

                var adminRole = roles.First(r => r.Name == RoleNames.Admin);
                var userRoles = context.Set<IdentityUserRole<Guid>>();
                userRoles.Add(new IdentityUserRole<Guid>
                {
                    UserId = user.Id,
                    RoleId = adminRole.Id
                });
            }

            context.SaveChanges();
        });

    private static IdentitySeedData GetSeedData()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                               .AddJsonFile("appsettings.Development.json", true)
                                               .AddJsonFile("appsettings.Production.json", true)
                                               .Build();
        var section = config.GetRequiredSection("SeedData:Identity");
        return section.Get<IdentitySeedData>() ??
               throw new InvalidOperationException("Seed data missing from configuration.");
    }
}