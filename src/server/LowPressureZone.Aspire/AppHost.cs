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
                       .WithBindMount($"{bindMountDir}/azuracast/stations",
                                      "/var/azuracast/stations")
                       .WithBindMount($"{bindMountDir}/azuracast/backups",
                                      "/var/azuracast/backups")
                       .WithBindMount($"{bindMountDir}/azuracast/database",
                                      "/var/lib/mysql")
                       .WithBindMount($"{bindMountDir}/azuracast/uploads",
                                      "/var/lib/azuracast/storage/uploads")
                       .WithHttpEndpoint(8147, 80, "Web")
                       .WithHttpEndpoint(8030, 8030, "Streaming")
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
                 .WaitFor(azuracast)
                 .WaitFor(mailpit)
                 .WaitForCompletion(migrations)
                 .WithReference(domainDatabase, "Data")
                 .WithReference(identityDatabase, "Identity");

// Runs on 4001, the argument pass to yarn is broken in Aspire.
// http://localhost:4001
var client = builder.AddViteApp("lpz-client", "../../client")
                    .WithYarn()
                    .WithRunScript("dev")
                    .WithHttpEndpoint(port: 4001, env: "PORT", name: "Client");

builder.Build().Run();