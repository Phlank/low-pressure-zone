using FluentValidation.Results;
using LowPressureZone.Core;

namespace LowPressureZone.Infrastructure.Abstraction;

public interface IConverter<in T1, T2>
{
    Result<T2, ValidationFailure> Convert(T1 instance);
}