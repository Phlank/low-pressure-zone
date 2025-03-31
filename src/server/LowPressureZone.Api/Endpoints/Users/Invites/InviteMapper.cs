using FastEndpoints;
using LowPressureZone.Identity.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class InviteMapper : IRequestMapper, IResponseMapper
{
    public Invitation<Guid, AppUser> ToEntity(InviteRequest request)
        => new Invitation<Guid, AppUser>()
        {
            UserId = Guid.NewGuid(),
            InvitationDate = DateTime.UtcNow,
            LastSentDate = DateTime.UtcNow
        };

    public InviteResponse FromEntity(Invitation<Guid, AppUser> invitation, Guid communityId)
    {
        invitation.User.ShouldNotBeNull();
        invitation.User.Email.ShouldNotBeNull();

        return new InviteResponse
        {
            Id = invitation.Id,
            CommunityId = communityId,
            InvitedAt = invitation.InvitationDate,
            LastSentAt = invitation.LastSentDate,
            Email = invitation.User.Email,
            DisplayName = invitation.User.DisplayName
        };
    }
}
