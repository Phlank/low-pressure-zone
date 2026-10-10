using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.CommunityAggregate.Events;
using LowPressureZone.Domain.CommunityAggregate.RelationshipEntity;
using LowPressureZone.Domain.CommunityAggregate.Rules;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Domain.CommunityAggregate;

public class Community : Entity
{
    [MaxLength(128)]
    public string Name
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;
    
    [MaxLength(512)]
    public string SocialUrl
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    public bool IsDeleted { get; private set; }
    public List<Relationship> Relationships { get; private init; } = [];

    // EF Core constructor
    private Community()
    {
    }

    private Community(string name, string socialUrl)
    {
        Name = name;
        SocialUrl = socialUrl;
        IsDeleted = false;
    }

    public static DomainResult<Community> Create(string name, string socialUrl) =>
        Rule.ApplyIntoResult(new Community(name, socialUrl),
                                          new NameIsRequiredRule(name),
                                          new NameLengthCannotExceed128Rule(name),
                                          new SocialUrlMustBeWellFormedRule(socialUrl),
                                          new SocialUrlLengthCannotExceed512Rule(socialUrl));

    public DomainResult<NoValue> Rename(string name)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                                       new NameChanged(Id),
                                                       new NameIsRequiredRule(name),
                                                       new NameLengthCannotExceed128Rule(name));
        
        if (result.IsSuccess)
            Name = name;

        return result;
    }

    public DomainResult<NoValue> ChangeSocialUrl(string socialUrl)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                                       new SocialUrlChanged(Id),
                                                       new SocialUrlLengthCannotExceed512Rule(socialUrl),
                                                       new SocialUrlMustBeWellFormedRule(socialUrl));
        if (result.IsSuccess)
            SocialUrl = socialUrl;

        return result;
    }

    public DomainResult<NoValue> SetRolesForUser(Guid userId, bool isPerformer, bool isOrganizer)
    {
        GetRelationshipForUser(userId).SetRoles(isPerformer, isOrganizer);
        return Rule.ApplyIntoResult(NoValue.Instance, new RelationshipChanged(Id, userId));
    }

    private Relationship GetRelationshipForUser(Guid userId)
    {
        var relationship = Relationships.FirstOrDefault(r => r.UserId == userId);
        if (relationship is null)
        {
            relationship = Relationship.Create(Id, userId, false, false).Value;
            Relationships.Add(relationship);
        }

        return relationship;
    }

    public DomainResult<NoValue> Delete()
    {
        IsDeleted = true;
        return DomainResult.Ok();
    }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Community>().HasIndex(nameof(Name)).IsUnique();
        modelBuilder.Entity<Community>().HasIndex(nameof(SocialUrl)).IsUnique();
        modelBuilder.Entity<Community>()
                    .HasMany(c => c.Relationships)
                    .WithOne(r => r.Community)
                    .HasForeignKey(r => r.CommunityId);
    }
}