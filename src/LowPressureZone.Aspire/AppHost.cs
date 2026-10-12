using LowPressureZone.Aspire.Extensions;
using Projects;
#pragma warning disable ASPIRECERTIFICATES001

// ReSharper disable UnusedVariable

const string bindMountDir = "../../tools/mounts";

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("lpz-postgres", port: 4000)
                      .WithDataVolume("lpz-data"); 
var domainDatabase = postgres.AddDatabase("lpz-domain");
var identityDatabase = postgres.AddDatabase("lpz-identity");
var pgAdmin = postgres.WithPgAdmin(cfg => { cfg.WithHostPort(4002); }, 
                                   containerName: "lpz-pgAdmin");

var azuracast = builder.AddContainer("azuracast", "ghcr.io/azuracast/azuracast", "0.23.8")
                       .WithBindMount($"{bindMountDir}/azuracast/stations",
                                      "/var/azuracast/stations",
                                      isReadOnly: false)
                       .WithBindMount($"{bindMountDir}/azuracast/backups",
                                      "/var/azuracast/backups",
                                      isReadOnly: false)
                       .WithBindMount($"{bindMountDir}/azuracast/mysql",
                                      "/var/lib/mysql",
                                      isReadOnly: false)
                       .WithBindMount($"{bindMountDir}/azuracast/uploads",
                                      "/var/lib/azuracast/storage/uploads",
                                      isReadOnly: false)
                       .WithHttpEndpoint(8147, 80, "Web", isProxied: false)
                       .WithHttpEndpoint(8030, 8030, "Streaming", isProxied: false)
                       .WithHttpEndpoint(8020, 8020, "Broadcasting", isProxied: false)
                       .WithEndpoint(8149, 2022, name: "SFTP", scheme: "sftp", isExternal: true)
                       .WithExternalHttpEndpoints()
                       .WithEnvironment(environment =>
                       {
                           environment.EnvironmentVariables.Add("MARIADB_AUTO_UPGRADE", "1");
                           environment.EnvironmentVariables.Add("MARIADB_RANDOM_ROOT_PASSWORD", "1");
                       });

var icecast = builder.AddContainer("icecast", "deepcomp/icecast2", "2.4.4")
                     .WithBindMount($"{bindMountDir}/icecast2/icecast.xml",
                                    "/etc/icecast2/icecast.xml")
                     .WithBindMount($"{bindMountDir}/icecast2/log",
                                    "/var/log/icecast2")
                     .WithBindMount($"{bindMountDir}/icecast2/mime.types",
                                    "/etc/mime.types")
                     .WithHttpEndpoint(8000, 8000, "icecast");

var mailpit = builder.AddMailPit("mailpit", 9280, 9281);

var api = builder.AddProject<LowPressureZone_Api>("lpz-api")
                 .AddConfigurationToEnvironment(builder.Configuration.GetSection("LowPressureZone"))
                 .WaitFor(azuracast)
                 .WaitFor(mailpit)
                 .WaitFor(domainDatabase)
                 .WaitFor(identityDatabase)
                 .WithReference(mailpit)
                 .WithReference(domainDatabase, "Data")
                 .WithReference(identityDatabase, "Identity");

var client = builder.AddViteApp("lpz-client", "../low-pressure-zone-client")
                    .WithYarn()
                    .WithHttpEndpoint(port: 4001, targetPort: 4001, env: "PORT", isProxied: false);

await builder.Build().RunAsync();