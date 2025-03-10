using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsersRequestValidator : Validator<GetUsersRequest>
{
    public GetUsersRequestValidator()
    {
        RuleFor(r => r.Roles).Must(roles => (roles ?? new List<string>()).All(role => RoleNames.All.Contains(role)))
                             .WithMessage(Errors.InvalidRole);
    }
}
