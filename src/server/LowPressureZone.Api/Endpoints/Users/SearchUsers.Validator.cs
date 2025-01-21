using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Users;

public class SearchUsersValidator : Validator<SearchUsersRequest>
{
    public SearchUsersValidator()
    {
        RuleFor(r => r.Username).NotEmpty().When(r => string.IsNullOrEmpty(r.Email), ApplyConditionTo.CurrentValidator)
                                .Empty().When(r => !string.IsNullOrEmpty(r.Email), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.Email).NotEmpty().When(r => string.IsNullOrEmpty(r.Username), ApplyConditionTo.CurrentValidator)
                             .Empty().When(r => !string.IsNullOrEmpty(r.Username), ApplyConditionTo.CurrentValidator);
    }
}
