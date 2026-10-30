using LowPressureZone.Domain.NewsAggregate;
using LowPressureZone.Domain.Settings;
using LowPressureZone.Scheduling.Domain.BroadcastAggregate;
using LowPressureZone.Scheduling.Domain.CommunityAggregate;
using LowPressureZone.Scheduling.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Scheduling.Domain.PerformerAggregate;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.ClashSlotEntity;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Broadcast> Broadcasts { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Performer> Performers { get; set; }
    public DbSet<Community> Communities { get; set; }

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
        
        Community.OnModelCreating(modelBuilder);
        Relationship.OnModelCreating(modelBuilder);
        
        Schedule.OnModelCreating(modelBuilder);
        HourlySlot.OnModelCreating(modelBuilder);
        ClashSlot.OnModelCreating(modelBuilder);
        

        modelBuilder.Entity<Setting>().HasIndex(nameof(Setting.Key)).IsUnique();
    }
}