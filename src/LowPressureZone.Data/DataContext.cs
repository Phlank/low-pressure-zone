using LowPressureZone.Data.Extensions;
using LowPressureZone.Domain.BroadcastAggregate;
using LowPressureZone.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Domain.NewsAggregate;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using HourlySlot = LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlot;

namespace LowPressureZone.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Broadcast> Broadcasts { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Performer> Performers { get; set; }
    public DbSet<Domain.CommunityAggregate.Community> Communities { get; set; }
    public IQueryable<Relationship> Relationships => Set<Relationship>().AsQueryable()
                                                                        .AsNoTracking();
    public DbSet<Schedule> Schedules { get; set; }
    public IQueryable<HourlySlot> HourlySlots => Set<HourlySlot>().AsQueryable()
                                                                  .AsNoTracking();
    public IQueryable<ClashSlot> ClashSlots => Set<ClashSlot>().AsQueryable()
                                                               .AsNoTracking();
    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Broadcast.OnModelCreating(modelBuilder);
        Schedule.OnModelCreating(modelBuilder);
        HourlySlot.OnModelCreating(modelBuilder);
        ClashSlot.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Setting>()
                    .HasIndex(nameof(Setting.Key)).IsUnique();
    }
}