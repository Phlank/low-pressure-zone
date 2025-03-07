﻿using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class GetResendInviteRequestValidator : Validator<GetResendInviteRequest>
{
    public GetResendInviteRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().WithMessage(Errors.Required).EmailAddress().WithMessage(Errors.InvalidEmail);
    }
}
