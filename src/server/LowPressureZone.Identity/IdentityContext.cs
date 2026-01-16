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
            Seed_AddRoles(context);
            Seed_AddAdminUser(context);
        }).UseAsyncSeeding(async (context, _, ct) =>
        {
            await Seed_AddRolesAsync(context, ct);
            await Seed_AddAdminUserAsync(context, ct);
        });

    private static void Seed_AddAdminUser(DbContext context)
    {
        var users = context.Set<AppUser>();
        var roles = context.Set<AppRole>();
        if (!users.Any())
        {
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
    }
    
    private static async Task Seed_AddAdminUserAsync(DbContext context, CancellationToken ct)
    {
        var users = context.Set<AppUser>();
        var roles = context.Set<AppRole>();
        if (!await users.AnyAsync(ct))
        {
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

        await context.SaveChangesAsync(ct);
    }

    private static void Seed_AddRoles(DbContext context)
    {
        var roles = context.Set<AppRole>();
        foreach (var roleName in RoleNames.DatabaseRoles)
        {
            if (roles.Any(r => r.Name == roleName)) continue;
            roles.Add(new AppRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
        }

        context.SaveChanges();
    }

    private static async Task Seed_AddRolesAsync(DbContext context, CancellationToken ct)
    {
        var roles = context.Set<AppRole>();
        foreach (var roleName in RoleNames.DatabaseRoles)
        {
            if (await roles.AnyAsync(r => r.Name == roleName, ct)) continue;
            roles.Add(new AppRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
        }

        await context.SaveChangesAsync(ct);
    }

    private static IdentitySeedData GetSeedData()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                               .AddJsonFile("appsettings.Development.json", true)
                                               .AddJsonFile("appsettings.Production.json", true)
                                               .Build();
        var section = config.GetSection("SeedData:Identity");
        return section.Get<IdentitySeedData>() ?? new IdentitySeedData
        {
            AdminDisplayName = "Admin",
            AdminUsername = "admin",
            AdminEmail = "email@none.com",
            AdminPassword = "ChangeMe123!"
        };
    }
}