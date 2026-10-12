using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

public class ClashSlotValidator : Validator<ClashSlotRequest>
{
    public ClashSlotValidator()
    {
        // This is to enforce what the frontend can currently support, but this can be adjusted in the future.
        // It stays out of the domain so that the domain doesn't get hung up on the incapabilities of the FE.
        RuleFor(x => x.Rounds).Must(x => x.Count == 3).WithMessage("Must have 3 rounds");
        RuleFor(x => x.Duration).Must(x => x == 2).WithMessage("Must be 2 hours");
    }
}