using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.PerformerAggregate.Rules;

namespace LowPressureZone.Domain.PerformerAggregate;

[Table("Performers")]
public sealed class Performer : Entity
{
    [MaxLength(64)]
    public string Name
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    [MaxLength(512)]
    public string? SocialUrl
    {
        get;
        private set => field = (value ?? "").Trim();
    }

    public Guid CreatorUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    // EF Core constructor
    private Performer()
    {
    }

    private Performer(Guid creatorUserId, string name, string? socialUrl, Guid id)
    {
        CreatorUserId = creatorUserId;
        Name = name;
        SocialUrl = socialUrl;
        Id = id;
    }

    public static DomainResult<Performer> Create(Guid creatorUserId, string name, string? socialUrl, Guid? id = null) =>
        Rule.ApplyIntoResult(new Performer(creatorUserId, name, socialUrl, id ?? Guid.NewGuid()),
                             new NameIsRequiredRule(name),
                             new NameLengthCannotExceed64Rule(name),
                             new SocialUrlLengthCannotExceed512Rule(socialUrl),
                             new SocialUrlMustBeWellFormedIfProvidedRule(socialUrl));

    public DomainResult<NoValue> ChangeName(string name)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new NameIsRequiredRule(name),
                                          new NameLengthCannotExceed64Rule(name));
        if (result.IsSuccess) Name = name;
        return result;
    }

    public DomainResult<NoValue> ChangeSocialUrl(string? socialUrl)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                          new SocialUrlLengthCannotExceed512Rule(socialUrl),
                                          new SocialUrlMustBeWellFormedIfProvidedRule(socialUrl));
        if (result.IsSuccess) SocialUrl = socialUrl;
        return result;
    }

    public DomainResult<NoValue> Delete()
    {
        IsDeleted = true;
        return DomainResult.Ok();
    }
}