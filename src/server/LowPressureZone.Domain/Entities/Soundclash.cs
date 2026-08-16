using LowPressureZone.Domain.Interfaces;
using LowPressureZone.Domain.PerformerAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.Entities;

public sealed class Soundclash : BaseEntity, IDateTimeRange
{
    public required Guid ScheduleId { get; set; }
    public Schedule Schedule { get; set; } = null!;
    public required Guid PerformerOneId { get; set; }
    public Performer PerformerOne { get; set; } = null!;
    public required Guid PerformerTwoId { get; set; }
    public Performer PerformerTwo { get; set; } = null!;
    public required string RoundOne { get; set; }
    public required string RoundTwo { get; set; }
    public required string RoundThree { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Soundclash>().HasIndex(nameof(StartsAt)).IsUnique();
        modelBuilder.Entity<Soundclash>().HasIndex(nameof(EndsAt)).IsUnique();

        modelBuilder.Entity<Soundclash>()
                    .HasOne(s => s.PerformerOne)
                    .WithMany()
                    .HasForeignKey(s => s.PerformerOneId)
                    .ExcludeForeignKeyFromMigrations();

        modelBuilder.Entity<Soundclash>()
                    .HasOne(s => s.PerformerTwo)
                    .WithMany()
                    .HasForeignKey(s => s.PerformerTwoId)
                    .ExcludeForeignKeyFromMigrations();
    }
}