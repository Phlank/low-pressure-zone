namespace LowPressureZone.Aspire.Extensions;

public static class EndpointsExtensions
{
    extension(IEnumerable<EndpointReference> endpoints)
    {
        public string GetUrl(string endpointName) =>
            endpoints.First(endpoint => endpoint.EndpointName == endpointName).Url;
    }
}