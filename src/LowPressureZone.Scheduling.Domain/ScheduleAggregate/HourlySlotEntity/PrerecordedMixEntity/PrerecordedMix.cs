using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core.Domain;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixEntity.Rules;
using LowPressureZone.Scheduling.Domain.UploadedMediaAggregate;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixEntity;

public class PrerecordedMix : Entity
{
    public Guid UploadedMediaId { get; private init; }
    public UploadedMedia? UploadedMedia { get; private init; }

    [MaxLength(2048)]
    public string UploadedFileName
    {
        get;
        private init => field = value.Trim();
    } = string.Empty;

    public HourlySlot? Slot { get; private init; }

    // EF Core constructor
    private PrerecordedMix()
    {
    }

    public static DomainResult<PrerecordedMix> Create(Guid uploadedMediaId, string uploadedFileName)
    {
        var prerecordedMix = new PrerecordedMix
        {
            UploadedMediaId = uploadedMediaId,
            UploadedFileName = uploadedFileName,
        };

        return Rule.ApplyIntoResult(prerecordedMix,
                                    new UploadedFileNameIsRequiredRule(prerecordedMix),
                                    new UploadedFileNameLengthMaxIs2048Rule(prerecordedMix));
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PrerecordedMix>().Property(mix => mix.Id).ValueGeneratedNever();
        
        modelBuilder.Entity<PrerecordedMix>().HasOne(mix => mix.UploadedMedia).WithOne()
                    .HasForeignKey<PrerecordedMix>(mix => mix.UploadedMediaId)
                    .ExcludeForeignKeyFromMigrations();
        modelBuilder.Entity<PrerecordedMix>().Navigation(mix => mix.UploadedMedia).AutoInclude();
    }
}