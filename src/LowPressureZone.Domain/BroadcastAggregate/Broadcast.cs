using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.BroadcastAggregate.BroadcastTimeValueObject;
using LowPressureZone.Domain.BroadcastAggregate.Rules;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.BroadcastAggregate;

public class Broadcast : Entity
{
    public int AzuraCastBroadcastId { get; private set; }
    public int AzuraCastStreamerId { get; private set; }
    public string AzuraCastStreamerDisplayName { get; private set; } = string.Empty;
    public bool HasFile { get; private set; }
    public bool IsArchived { get; private set; }
    public BroadcastTime Time { get; private set; }

    // EF Core constructor
    private Broadcast()
    {
    }

    public static DomainResult<Broadcast> Create(int azuraCastBroadcastId,
                                                 int azuraCastStreamerId,
                                                 string azuraCastStreamerDisplayName,
                                                 bool hasFile,
                                                 DateTimeOffset startsAt,
                                                 DateTimeOffset? endsAt)
    {
        var timeResult = BroadcastTime.Create(startsAt, endsAt);
        var errors = Rule.Apply(new BroadcastIdMustBeAboveZeroRule(azuraCastBroadcastId),
                                new StreamerIdMustBeAboveZeroRule(azuraCastStreamerId))
                         .Concat(timeResult.Errors).ToList();

        if (errors.Count > 0)
        {
            return DomainResult.Err<Broadcast>(errors);
        }

        return DomainResult.Ok(new Broadcast
        {
            AzuraCastBroadcastId = azuraCastBroadcastId,
            AzuraCastStreamerId = azuraCastStreamerId,
            AzuraCastStreamerDisplayName = azuraCastStreamerDisplayName,
            HasFile = hasFile,
            IsArchived = false,
            Time = timeResult.Value
        });
    }

    public DomainResult<NoValue> Archive()
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new CannotArchiveIfNoFileRule(this),
                                          new CannotArchiveIfAlreadyArchivedRule(this));
        if (result.IsSuccess)
        {
            IsArchived = true;
        }

        return result;
    }

    public DomainResult<NoValue> SetEnd(DateTimeOffset endsAt)
    {
        var timeResult = BroadcastTime.Create(Time.StartsAt, endsAt);
        if (timeResult.IsSuccess)
        {
            Time = timeResult.Value;
        }

        return timeResult.ToNoValue();
    }

    public DomainResult<NoValue> SetHasFile(bool hasFile)
    {
        HasFile = hasFile;
        return DomainResult.Ok();
    }

    public DomainResult<NoValue> SetDisplayName(string streamerDisplayName)
    {
        AzuraCastStreamerDisplayName = streamerDisplayName;
        return DomainResult.Ok();
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Broadcast>()
                    .HasIndex(broadcast => broadcast.AzuraCastBroadcastId)
                    .IsUnique();
        
        modelBuilder.Entity<Broadcast>()
                    .ComplexProperty(broadcast => broadcast.Time);
    }
}