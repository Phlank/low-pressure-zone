using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Audience> Audiences { get; set; }
    public DbSet<Performer> Performers { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Timeslot> Timeslots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Audience>().HasIndex(nameof(Audience.Name)).IsUnique();

        modelBuilder.Entity<Performer>().HasIndex(nameof(Performer.Name)).IsUnique();

        modelBuilder.Entity<Schedule>().HasIndex(nameof(Schedule.StartsAt)).IsUnique();
        modelBuilder.Entity<Schedule>().HasIndex(nameof(Schedule.EndsAt)).IsUnique();

        modelBuilder.Entity<Timeslot>().HasIndex(nameof(Timeslot.StartsAt)).IsUnique();
        modelBuilder.Entity<Timeslot>().HasIndex(nameof(Timeslot.EndsAt)).IsUnique();
    }
}
