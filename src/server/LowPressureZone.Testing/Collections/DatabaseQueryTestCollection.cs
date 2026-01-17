using LowPressureZone.Testing.Infrastructure.Fixtures;
using Xunit;

namespace LowPressureZone.Testing.Collections;

[CollectionDefinition("Database Query Tests")]
public class DatabaseQueryTestCollection : ICollectionFixture<DatabaseFixture> {}