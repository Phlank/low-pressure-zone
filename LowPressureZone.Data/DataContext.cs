using LowPressureZone.Data.Extensions;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Entities.Settings;
using LowPressureZone.Domain.NewsAggregate;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Broadcast> Broadcasts { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Performer> Performers { get; set; }
    public DbSet<Domain.CommunityAggregate.Community> Communities { get; set; }
    public DbSet<Domain.ScheduleAggregate.Schedule> Schedules { get; set; }
    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Domain.ScheduleAggregate.Schedule.OnModelCreating(modelBuilder);
        HourlySlot.OnModelCreating(modelBuilder);
        ClashSlot.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Setting>()
                    .HasIndex(nameof(Setting.Key)).IsUnique();
    }
}