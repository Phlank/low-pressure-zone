using Amazon;
using Aspire.Hosting.LocalStack.Container;
using LocalStack.Client.Options;
using LowPressureZone.Aspire.Extensions;
using LowPressureZone.Aspire.Extensions.LocalStack;
using Microsoft.Extensions.Configuration;
using Projects;

#pragma warning disable ASPIRECERTIFICATES001

// ReSharper disable UnusedVariable

var builder = DistributedApplication.CreateBuilder(args);
var bindMountDirectory = Path.GetFullPath(builder.Configuration.GetValue<string>("AppHost:BindMountDirectory")!);

var postgres = builder.AddPostgres("lpz-postgres", port: builder.Configuration.GetValue<int>("AppHost:Ports:Postgres")!)
                      .WithDataVolume("lpz-data");
var domainDatabase = postgres.AddDatabase("lpz-domain");
var identityDatabase = postgres.AddDatabase("lpz-identity");
var pgAdmin = postgres.WithPgAdmin(cfg =>
                                   {
                                       cfg.WithHostPort(builder.Configuration.GetValue<int>("AppHost:Ports:PgAdmin")!);
                                   },
                                   containerName: "lpz-pgAdmin");

var azuracast = builder.AddContainer("azuracast", "ghcr.io/azuracast/azuracast", "0.23.9")
                       .WithBindMount($"{bindMountDirectory}/azuracast/stations",
                                      "/var/azuracast/stations",
                                      isReadOnly: false)
                       .WithBindMount($"{bindMountDirectory}/azuracast/backups",
                                      "/var/azuracast/backups",
                                      isReadOnly: false)
                       .WithBindMount($"{bindMountDirectory}/azuracast/mysql",
                                      "/var/lib/mysql",
                                      isReadOnly: false)
                       .WithBindMount($"{bindMountDirectory}/azuracast/uploads",
                                      "/var/lib/azuracast/storage/uploads",
                                      isReadOnly: false)
                       .WithHttpEndpoint(8650, 80, "Web", isProxied: false)
                       .WithHttpEndpoint(8651, 8030, "Streaming", isProxied: false)
                       .WithHttpEndpoint(8652, 8020, "Broadcasting", isProxied: false)
                       .WithEndpoint(8653, 2022, name: "SFTP", scheme: "sftp", isExternal: true, isProxied: false)
                       .WithExternalHttpEndpoints()
                       .WithEnvironment(environment =>
                       {
                           environment.EnvironmentVariables.Add("MARIADB_AUTO_UPGRADE", "1");
                           environment.EnvironmentVariables.Add("MARIADB_RANDOM_ROOT_PASSWORD", "1");
                       });

var icecast = builder.AddContainer("icecast", "deepcomp/icecast2", "2.4.4")
                     .WithBindMount($"{bindMountDirectory}/icecast2/icecast.xml",
                                    "/etc/icecast2/icecast.xml")
                     .WithBindMount($"{bindMountDirectory}/icecast2/log",
                                    "/var/log/icecast2")
                     .WithBindMount($"{bindMountDirectory}/icecast2/mime.types",
                                    "/etc/mime.types")
                     .WithHttpEndpoint(8647, 8000, "icecast", isProxied: false);

var mailpit = builder.AddMailPit("mailpit",
                                 builder.Configuration.GetValue<int>("AppHost:Ports:MailPitHttp")!,
                                 builder.Configuration.GetValue<int>("AppHost:Ports:MailPitSmtp")!);

var awsConfig = builder.AddAWSSDKConfig()
                       .WithProfile("default")
                       .WithRegion(RegionEndpoint.USEast1);

var localStackOptions = new LocalStackOptions(useLocalStack: true,
                                              sessionOptions: new SessionOptions(),
                                              configOptions: new ConfigOptions(localStackHost: "localhost",
                                                                               useSsl: false,
                                                                               useLegacyPorts: false,
                                                                               edgePort: builder.Configuration
                                                                                                .GetValue<
                                                                                                    int>("AppHost:Ports:LocalStack")
                                                                                                !));

var localStackPort = builder.Configuration.GetValue<int>("AppHost:Ports:LocalStack")!;
var localStack = builder.AddLpzLocalStack();

var api = builder.AddProject<LowPressureZone_Api>("lpz-api")
                 .AddConfigurationToEnvironment(builder.Configuration.GetSection("LowPressureZone"))
                 .WaitFor(azuracast)
                 .WaitFor(mailpit)
                 .WaitForStart(domainDatabase)
                 .WaitForStart(identityDatabase)
                 .WaitForStart(localStack.)
                 .WaitForCompletion(s3Seed)
                 .WithReference(mailpit)
                 .WithReference(domainDatabase, "Data")
                 .WithReference(identityDatabase, "Identity")
                 .WithEnvironment("AWSCONFIG__ENDPOINT", localStackEndpoint);

var clientPort = builder.Configuration.GetValue<int>("AppHost:Ports:Client")!;
var client = builder.AddViteApp("lpz-client", "../low-pressure-zone-client")
                    .WithYarn()
                    .WithHttpEndpoint(port: clientPort,
                                      targetPort: clientPort,
                                      env: "PORT",
                                      isProxied: false);

await builder.Build().RunAsync();