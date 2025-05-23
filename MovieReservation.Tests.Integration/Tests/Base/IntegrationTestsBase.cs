using MovieReservation.Tests.Integration.Factories;
using MovieReservation.Tests.Integration.Fixtures;

namespace MovieReservation.Tests.Integration.Tests.Base;
 
public abstract class IntegrationTestsBase : IDisposable
{
	protected readonly DatabaseFixture _dbFixture;
	protected readonly HttpClient client;

	protected IntegrationTestsBase(CustomWebApplicationFactory webAppFixture, DatabaseFixture dbFixture)
	{
		_dbFixture = dbFixture;

		client = webAppFixture.CreateClient();
	}

    public void Dispose()
    {
		GC.SuppressFinalize(this);

		_dbFixture.Cleanup();
	}
}