using FluentValidation.Results;

namespace LowPressureZone.Core.Interfaces;

public interface IAsyncConverter<in TSource, TDestination> where TSource : notnull where TDestination : notnull
{
    Task<Result<TDestination, ValidationFailure>> ConvertAsync(TSource source, CancellationToken ct = default);
}