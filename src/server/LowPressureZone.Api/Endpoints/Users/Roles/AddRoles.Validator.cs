using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Users.Roles;

public class AddRolesValidator : Validator<AddRolesRequest>
{
    public AddRolesValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Roles).NotEmpty().ForEach(role => role.Must(role => RoleNames.AllRoles.Contains(role)).WithMessage($"Only roles {string.Join(", ", RoleNames.AllRoles)} are allowed"));
    }
}
