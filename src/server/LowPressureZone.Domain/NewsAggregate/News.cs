using System.ComponentModel.DataAnnotations;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.NewsAggregate.Rules;

namespace LowPressureZone.Domain.NewsAggregate;

public sealed class News : Entity
{
    [MaxLength(256)]
    public string Title
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    [MaxLength(16384)]
    public string Content
    {
        get;
        private set => field = value.Trim();
    } = string.Empty;

    public DateTimeOffset PublishedAt { get; private init; } = DateTimeOffset.UtcNow;

    // EF Core constructor
    private News()
    {
    }

    private News(string title, string content)
    {
        Title = title;
        Content = content;
    }

    public static DomainResult<News> Create(string title, string content) =>
        Rule.ApplyIntoResult(new News(title, content),
                                          new TitleIsRequiredRule(title),
                                          new TitleLengthCannotExceedThan256Rule(title),
                                          new ContentIsRequiredRule(content),
                                          new ContentLengthCannotExceed16384Rule(content));

    public DomainResult<NoValue> Edit(string title, string content)
    {
        var result = Rule.ApplyIntoResult(NoValue.Instance,
                                                       new TitleIsRequiredRule(title),
                                                       new TitleLengthCannotExceedThan256Rule(title),
                                                       new ContentIsRequiredRule(content),
                                                       new ContentLengthCannotExceed16384Rule(content));

        if (result.IsSuccess)
        {
            Title = title;
            Content = content;
        }

        return result;
    }
}