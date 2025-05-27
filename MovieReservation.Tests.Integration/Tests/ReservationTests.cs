using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Serialization;
using MovieReservation.Tests.Integration.Factories;
using MovieReservation.Tests.Integration.Fixtures;
using MovieReservation.Tests.Integration.TestCollections;
using MovieReservation.Tests.Integration.Tests.Base;
using MovieReservation.Application.Utility;
using Microsoft.EntityFrameworkCore;
using MovieReservation.API.Requests.Reservation;

namespace MovieReservation.Tests.Integration;

[Collection(nameof(DatabaseFixtureCollection))]
public class ReservationTests : IntegrationTestsBase, IClassFixture<CustomWebApplicationFactory>
{
    public ReservationTests(CustomWebApplicationFactory webAppFixture, DatabaseFixture dbFixture)
		: base(webAppFixture, dbFixture)
	{
		using var context = dbFixture.Context;

		var movies = JsonReader.ReadAndDeserialize<Movie>(PathHelper.GetAbsolutePath("Tests/TestData/Movies.json"));
		var showtimes = JsonReader.ReadAndDeserialize<Showtime>(PathHelper.GetAbsolutePath("Tests/TestData/Showtimes.json"));

		context.AddRange(movies);
		context.AddRange(showtimes);
		context.SaveChanges();
	}

	[Fact]
	public async Task Create_Should_ReturnSuccess()
	{
		// Arrange
		using var context = _dbFixture.Context;
		var testShowtime = context.Showtimes.AsNoTracking().First();
		var testSeats = context.Seats.AsNoTracking().Where(s => s.HallId == testShowtime.HallId).Take(2).ToList();

		CreateReservationRequest testRequest = new(testShowtime.Id, [testSeats[0].Id, testSeats[1].Id ]);

		// Act
		var response = await client.PostAsJsonAsync("api/reservation", testRequest);

		// Assert
		Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
	}

    [Fact]
	public async Task Delete_Should_ReturnsSuccesss()
	{
		// Arrange
		using var context = _dbFixture.Context;
		var user = context.Users.AsNoTracking().First();
		var testShowtime = context.Showtimes.First();
		var testSeats = context.Seats.Where(s => s.HallId == testShowtime.HallId).Take(2).ToList();

		Reservation testReservation = new("code", user.Id, testShowtime.Id) { Id = Guid.NewGuid() };
		foreach (var seat in testSeats)
            testReservation.SeatReservations.Add(new() { Seat = seat, Showtime = testShowtime });

		context.Reservations.Add(testReservation);
		context.SaveChanges();

		// Act
		var response = await client.DeleteAsync($"api/reservation/{testReservation.Id}");

		// Assert
		Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
	}
}