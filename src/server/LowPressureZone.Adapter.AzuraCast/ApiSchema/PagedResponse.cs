using System.Text.Json.Serialization;

namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public sealed class PagedResponse<T>
{
    public int Page { get; init; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; init; }

    public int Total { get; init; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }

    public Dictionary<string, string>? Links { get; init; }
    public T[] Rows { get; init; } = [];
}