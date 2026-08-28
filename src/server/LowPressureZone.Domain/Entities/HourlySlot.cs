using System.ComponentModel.DataAnnotations;
using LowPressureZone.Domain.Interfaces;
using LowPressureZone.Domain.PerformerAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.Entities;

public sealed class HourlySlot : BaseEntity, IDateTimeRange
{
    [MaxLength(64)] public string? Subtitle { get; set; }

    [MaxLength(64)] public required string Type { get; set; }

    public required Guid PerformerId { get; set; }
    public Performer Performer { get; set; } = null!;
    public required Guid ScheduleId { get; set; }

    public Schedule Schedule { get; init; } = null!;

    [MaxLength(1024)] public string? UploadedFileName { get; set; }

    public int? AzuraCastMediaId { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HourlySlot>().HasIndex(nameof(HourlySlot.StartsAt)).IsUnique();
        modelBuilder.Entity<HourlySlot>().HasIndex(nameof(HourlySlot.EndsAt)).IsUnique();

        modelBuilder.Entity<HourlySlot>()
                    .HasOne(t => t.Performer)
                    .WithMany()
                    .HasForeignKey(t => t.PerformerId)
                    .ExcludeForeignKeyFromMigrations();
    }
}