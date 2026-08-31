using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Domain.Settings;

public sealed class Setting : Entity
{
    public required SettingKey Key { get; set; }

    [Column(TypeName = "jsonb")]
    public required string Value { get; set; } = string.Empty;
}