using LowPressureZone.Testing.Infrastructure.Fixtures;
using Xunit;

namespace LowPressureZone.Testing.Collections;

[CollectionDefinition("DatabaseQueryTests")]
public class DatabaseQueryTestCollection : ICollectionFixture<DatabaseFixture> {}