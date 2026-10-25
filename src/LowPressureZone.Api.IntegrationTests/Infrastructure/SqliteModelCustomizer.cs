using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LowPressureZone.Api.IntegrationTests.Infrastructure;

/// <summary>
///     SQLite cannot compare or order <see cref="DateTimeOffset" /> values, which PostgreSQL handles natively.
///     Storing them as binary keeps queries such as <c>OrderBy(broadcast => broadcast.Time.StartsAt)</c> working.
/// </summary>
public sealed class SqliteModelCustomizer(ModelCustomizerDependencies dependencies)
    : RelationalModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder modelBuilder, DbContext context)
    {
        base.Customize(modelBuilder, context);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            ConvertDateTimeOffsets(entityType);
    }

    private static void ConvertDateTimeOffsets(IMutableTypeBase type)
    {
        foreach (var property in type.GetDeclaredProperties())
        {
            if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                property.SetValueConverter(new DateTimeOffsetToBinaryConverter());
        }

        foreach (var complexProperty in type.GetDeclaredComplexProperties())
            ConvertDateTimeOffsets(complexProperty.ComplexType);
    }
}
