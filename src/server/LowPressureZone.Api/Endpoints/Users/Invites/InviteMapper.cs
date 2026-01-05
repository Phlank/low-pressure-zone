using FastEndpoints;
using LowPressureZone.Identity.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public sealed class InviteMapper : IRequestMapper, IResponseMapper
{
    public Invitation<Guid, AppUser> ToEntity(InviteRequest request)
        => new()
        {
            UserId = Guid.NewGuid(),
            InvitationDate = DateTime.UtcNow,
            LastSentDate = DateTime.UtcNow
        };

    public InviteResponse FromEntity(Invitation<Guid, AppUser> invitation, IEnumerable<Guid> communityIds)
    {
        invitation.User.ShouldNotBeNull();
        invitation.User.Email.ShouldNotBeNull();

        return new InviteResponse
        {
            Id = invitation.Id,
            CommunityIds = communityIds,
            InvitedAt = invitation.InvitationDate,
            LastSentAt = invitation.LastSentDate,
            Email = invitation.User.Email,
            DisplayName = invitation.User.Email
        };
    }
}