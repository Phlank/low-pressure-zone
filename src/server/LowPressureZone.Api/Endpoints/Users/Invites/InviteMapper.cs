using FastEndpoints;
using LowPressureZone.Api.Endpoints.Users.Invite;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class InviteMapper() : Mapper<InviteRequest, InviteResponse, Invitation<Guid, AppUser>>, IRequestMapper, IResponseMapper
{
    public override Invitation<Guid, AppUser> ToEntity(InviteRequest req)
    {
        return new Invitation<Guid, AppUser>()
        {
            UserId = Guid.NewGuid(),
            InvitationDate = DateTime.UtcNow,
            LastSentDate = DateTime.UtcNow,
        };
    }

    public override Task<Invitation<Guid, AppUser>> ToEntityAsync(InviteRequest req, CancellationToken ct = default)
        => Task.FromResult(ToEntity(req));

    public override InviteResponse FromEntity(Invitation<Guid, AppUser> invitation)
    {
        invitation.User.ShouldNotBeNull();
        invitation.User.Email.ShouldNotBeNull();

        return new InviteResponse
        {
            Id = invitation.Id,
            InvitedAt = invitation.InvitationDate,
            LastSentAt = invitation.LastSentDate,
            Email = invitation.User.Email
        };
    }

    public override Task<InviteResponse> FromEntityAsync(Invitation<Guid, AppUser> invitation, CancellationToken ct = default)
        => Task.FromResult(FromEntity(invitation));
}
