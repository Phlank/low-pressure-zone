using LowPressureZone.Aspire.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Projects;

const string bindMountDir = "../../../tools/mounts";

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("lpz-postgres")
                      .WithDataVolume();
var domainDatabase = postgres.AddDatabase("lpz-domain");
var identityDatabase = postgres.AddDatabase("lpz-identity");
var pgAdmin = postgres.WithPgAdmin(containerName: "lpz-pgAdmin");

var migrations = builder.AddProject<LowPressureZone_Aspire_Migrations>("migrations")
                        .WaitFor(domainDatabase)
                        .WaitFor(identityDatabase)
                        .WithReference(domainDatabase, "Data")
                        .WithReference(identityDatabase, "Identity");

var azuracast = builder.AddContainer("azuracast", "ghcr.io/azuracast/azuracast", "0.23.2")
                       .WithBindMount(source: $"{bindMountDir}/azuracast/stations",
                                      target: "/var/azuracast/stations")
                       .WithBindMount(source: $"{bindMountDir}/azuracast/backups",
                                      target: "/var/azuracast/backups")
                       .WithBindMount(source: $"{bindMountDir}/azuracast/database",
                                      target: "/var/lib/mysql")
                       .WithBindMount(source: $"{bindMountDir}/azuracast/uploads",
                                      target: "/var/lib/azuracast/storage/uploads")
                       .WithHttpEndpoint(port: 8147, targetPort: 80, name: "Web")
                       .WithHttpEndpoint(port: 8030, targetPort: 8030, name: "Streaming")
                       .WithEndpoint(port: 8149, targetPort: 2022, name: "SFTP", scheme: "sftp", isExternal: true)
                       .WithExternalHttpEndpoints()
                       .WithEnvironment(environment =>
                       {
                           environment.EnvironmentVariables.Add("MARIADB_AUTO_UPGRADE", "1");
                           environment.EnvironmentVariables.Add("MARIADB_RANDOM_ROOT_PASSWORD", "1");
                       });

var icecast = builder.AddContainer("icecast", "deepcomp/icecast2", "2.4.4")
                     .WithBindMount(source: $"{bindMountDir}/icecast2/icecast.xml",
                                    target: "/etc/icecast2/icecast.xml")
                     .WithBindMount(source: $"{bindMountDir}/icecast2/log",
                                    target: "/var/log/icecast2")
                     .WithBindMount(source: $"{bindMountDir}/icecast2/mime.types",
                                    target: "/etc/mime.types")
                     .WithHttpEndpoint(port: 8000, targetPort: 8000, name: "icecast");

var mailpit = builder.AddMailPit("mailpit", 9280, 9281);

var api = builder.AddProject<LowPressureZone_Api>("lpz-api")
                 .WaitFor(azuracast)
                 .WaitFor(mailpit)
                 .WaitForCompletion(migrations)
                 .WithReference(domainDatabase, "Data")
                 .WithReference(identityDatabase, "Identity");

// Runs on 4001, the argument pass to yarn is broken in Aspire.
// http://localhost:4001
var client = builder.AddViteApp("lpz-client", "../../client")
                    .WithYarn()
                    .WaitFor(api);

builder.Build().Run();