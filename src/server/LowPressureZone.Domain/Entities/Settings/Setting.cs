using System.ComponentModel.DataAnnotations.Schema;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Domain.Entities.Settings;

public sealed class Setting : BaseEntity
{
    public required SettingKey Key { get; set; }

    [Column(TypeName = "jsonb")]
    public required string Value { get; set; } = string.Empty;
}