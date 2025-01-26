using LowPressureZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain;

public class DataContext : DbContext
{
    public DbSet<Page> Pages { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
}
