using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
}
