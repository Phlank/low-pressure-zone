using Xunit.Sdk;
using Xunit.v3;

// All test classes share one in-memory host, database, and AzuraCast fake, so they must not run concurrently.
[assembly: Parallelization(Mode = ParallelMode.None)]
