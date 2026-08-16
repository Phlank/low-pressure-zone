using LowPressureZone.Data.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Entities.Settings;
using LowPressureZone.Domain.NewsAggregate;
using LowPressureZone.Domain.PerformerAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Broadcast> Broadcasts { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityRelationship> CommunityRelationships { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Performer> Performers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Soundclash> Soundclashes { get; set; }
    public DbSet<Timeslot> Timeslots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Community>().HasIndex(nameof(Community.Name)).IsUnique();

        modelBuilder.Entity<Schedule>().HasIndex(nameof(Schedule.StartsAt)).IsUnique();
        modelBuilder.Entity<Schedule>().HasIndex(nameof(Schedule.EndsAt)).IsUnique();

        Timeslot.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CommunityRelationship>()
                    .HasIndex(nameof(CommunityRelationship.CommunityId), nameof(CommunityRelationship.UserId))
                    .IsUnique();

        modelBuilder.Entity<Setting>()
                    .HasIndex(nameof(Setting.Key)).IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        optionsBuilder.ConfigureDomainSeeding();
    }
}