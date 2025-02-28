using FastEndpoints;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerResponseMapper : ResponseMapper<PerformerResponse, Performer>
{
    private readonly PerformerRules _rules;

    public PerformerResponseMapper(PerformerRules rules)
    {
        _rules = rules;
    }

    public override PerformerResponse FromEntity(Performer p)
    {
        return new PerformerResponse
        {
            Id = p.Id,
            Name = p.Name,
            Url = p.Url,
            IsDeletable = _rules.CanUserDeletePerformer(p),
            IsEditable = _rules.CanUserEditPerformer(p),
            IsLinked = _rules.IsUserLinkedToPerformer(p),
        };
    }
}
