using System.Net;

namespace LowPressureZone.Infrastructure.Http;

public static class HttpStatusCodeExtensions
{
    public static bool IsSuccess(this HttpStatusCode code) => code is >= HttpStatusCode.OK and <= HttpStatusCode.Ambiguous;
    public static bool IsError(this HttpStatusCode code) => !code.IsSuccess();
}