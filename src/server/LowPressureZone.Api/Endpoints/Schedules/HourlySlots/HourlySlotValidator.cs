using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class HourlySlotValidator : Validator<HourlySlotRequest>
{
    public HourlySlotValidator()
    {
        RuleFor(x => x).Custom((x, ctx) =>
        {
            if (x.ReplaceMedia && x.File is null) ctx.AddFailure(nameof(x.File), "Required");
        });
    }
}