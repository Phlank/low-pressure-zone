﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix")]
public record StationQueue
{
    [JsonPropertyName("cued_at")]
    public int CuedAt { get; set; }

    [JsonPropertyName("played_at")]
    public int? PlayedAt { get; set; }

    public double Duration { get; set; }
    public string? Playlist { get; set; }

    [JsonPropertyName("is_request")]
    public bool IsRequest { get; set; }

    public required Song Song { get; set; }
}
