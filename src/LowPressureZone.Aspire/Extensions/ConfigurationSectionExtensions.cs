using Microsoft.Extensions.Configuration;

namespace LowPressureZone.Aspire.Extensions;

public static class ConfigurationSectionExtensions
{
    public static IEnumerable<KeyValuePair<string, string>> Flatten(this IConfigurationSection section, string? variablePrefix = null)
    {
        variablePrefix ??= "";
        var children = section.GetChildren().ToList();

        if (children.Count == 0)
        {
            yield return new KeyValuePair<string, string>(variablePrefix, section.Value ?? string.Empty);
            yield break;
        }

        foreach (var child in children)
        {
            var childPath = !string.IsNullOrWhiteSpace(variablePrefix) 
                                ? string.Concat(variablePrefix, "__", child.Key) 
                                : child.Key;

            foreach (var entry in child.Flatten(childPath))
            {
                yield return entry;
            }
        }
    }
}