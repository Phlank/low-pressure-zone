using FastEndpoints;
using FluentValidation;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class CommunityRelationshipsRequestValidator : Validator<CommunityRelationshipRequest>
{
    public CommunityRelationshipsRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(request => request).CustomAsync(async (request, context, ct) =>
        {
            var communityId = accessor.GetGuidRouteParameterOrDefault("communityId");
            var userId = accessor.GetGuidRouteParameterOrDefault("userId");
            var dataContext = Resolve<DataContext>();
            var existingRelationship = await dataContext.CommunityRelationships
                                                        .Where(relationship => relationship.CommunityId == communityId && relationship.UserId == userId)
                                                        .FirstOrDefaultAsync(ct);
            if (existingRelationship == null) return;
            if (accessor.HttpContext!.Request.Method == "POST")
                context.AddFailure("User already has a relationship with this community.");
        });
    }
}
