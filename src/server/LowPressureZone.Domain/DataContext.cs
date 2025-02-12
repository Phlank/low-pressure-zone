using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain;

public class DataContext : DbContext
{
    public DbSet<Audience> Audiences { get; set; }
    public DbSet<Performer> Performers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Timeslot> Timeslots { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Audience>().HasIndex(nameof(Audience.Name)).IsUnique();

        modelBuilder.Entity<Performer>().HasIndex(nameof(Performer.Name)).IsUnique();

        modelBuilder.Entity<Schedule>().HasIndex(nameof(Schedule.Start)).IsUnique();
        modelBuilder.Entity<Schedule>().HasIndex(nameof(Schedule.End)).IsUnique();

        modelBuilder.Entity<Timeslot>().HasIndex(nameof(Timeslot.Start)).IsUnique();
        modelBuilder.Entity<Timeslot>().HasIndex(nameof(Timeslot.End)).IsUnique();
    }
}
