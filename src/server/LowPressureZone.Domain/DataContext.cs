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
}
