using LowPressureZone.Api.Test.Infrastructure.Fixtures;
using Xunit;

namespace LowPressureZone.Api.Test.Collections;

[CollectionDefinition("DatabaseQueryTests")]
public class DatabaseQueryTestCollection : ICollectionFixture<DatabaseFixture> {}