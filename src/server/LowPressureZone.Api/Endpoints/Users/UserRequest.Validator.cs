using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Users;

public class UserRequestValidator : Validator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(req => req.Email).NotEmpty().EmailAddress();
    }
}
