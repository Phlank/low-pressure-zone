using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace LowPressureZone.Adapter.AzuraCast.Extensions;

public static class AzuraCastWebApplicationBuilderExtensions
{
    public static void AddAzuraCast(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AzuraCastClientConfiguration>(builder.Configuration
                                                                        .GetSection(AzuraCastClientConfiguration.Name));
        builder.Services.AddHttpClient("AzuraCastHttpClient", (services, client) =>
        {
            var configuration = services.GetRequiredService<IOptions<AzuraCastClientConfiguration>>().Value;
            client.BaseAddress = configuration.ApiUrl;
            client.DefaultRequestHeaders.Add("X-API-Key", configuration.ApiKey);
        });
        builder.Services.AddKeyedSingleton<ISftpClient>("AzuraCastSftpClient", (provider, key) =>
        {
            if (!key.Equals("AzuraCastSftpClient"))
                throw new ArgumentException($"Unsupported key '{key}' for ISftpClient registration.");
            
            var configuration = provider.GetRequiredService<IOptions<AzuraCastClientConfiguration>>().Value;
            return new SftpClient(configuration.SftpHost, 
                                  configuration.SftpPort, 
                                  configuration.SftpUser,
                                  configuration.SftpPassword);
        });
        builder.Services.AddSingleton<IAzuraCastClient, AzuraCastClient>();
    }
}