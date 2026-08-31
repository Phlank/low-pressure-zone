using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.BroadcastAggregate.Rules;

namespace LowPressureZone.Domain.BroadcastAggregate;

public class Broadcast : Entity
{
    public int AzuraCastBroadcastId { get; private set; }
    public bool IsArchived { get; private set; }

    // EF Core constructor
    private Broadcast()
    {
    }

    public DomainResult<Broadcast> Create(int azuraCastBroadcastId) => DomainResult.Ok(new Broadcast()
    {
        AzuraCastBroadcastId = azuraCastBroadcastId,
        IsArchived = false
    });
    
    public DomainResult<NoValue> Archive()
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new CannotArchiveIfAlreadyArchivedRule(this));
        if (result.IsSuccess)
        {
            IsArchived = true;
        }
        return result;
    }
}