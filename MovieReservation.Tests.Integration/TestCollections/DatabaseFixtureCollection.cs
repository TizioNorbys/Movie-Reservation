using MovieReservation.Tests.Integration.Fixtures;
namespace MovieReservation.Tests.Integration.TestCollections;

[CollectionDefinition(nameof(DatabaseFixtureCollection))]
public class DatabaseFixtureCollection : ICollectionFixture<DatabaseFixture>
{
}