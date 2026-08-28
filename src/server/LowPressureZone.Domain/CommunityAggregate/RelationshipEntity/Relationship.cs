using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;

namespace LowPressureZone.Domain.CommunityAggregate.RelationshipEntity;

[Table("CommunityRelationships")]
public class Relationship : Entity
{
    public Guid CommunityId { get; private init; }
    public Community Community { get; private set; } = null!;
    public Guid UserId { get; private init; }
    public bool IsOrganizer { get; private set; }
    public bool IsPerformer { get; private set; }

    // EF Core constructor
    private Relationship()
    {
    }

    private Relationship(Guid communityId, Guid userId, bool isOrganizer, bool isPerformer)
    {
        CommunityId = communityId;
        UserId = userId;
        IsOrganizer = isOrganizer;
        IsPerformer = isPerformer;
    }

    internal static DomainResult<Relationship> Create(Guid communityId, Guid userId, bool isOrganizer, bool isPerformer) =>
        DomainResult.Ok(new Relationship(communityId, userId, isOrganizer, isPerformer));

    internal DomainResult<NoValue> SetRoles(bool isPerformer, bool isOrganizer)
    {
        IsPerformer = isPerformer;
        IsOrganizer = isOrganizer;
        return DomainResult.Ok();
    }
}