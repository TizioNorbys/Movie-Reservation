using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Application.Constants;
using MovieReservation.Application.Utility;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Entities;
using MovieReservation.Infrastracture.Persistence;
using MovieReservation.Infrastracture.Serialization;

namespace MovieReservation.Tests.Integration.Fixtures;

public class DatabaseFixture
{
    private static readonly string connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS_TEST")!;

    public MovieDbContext Context => GetContext();

	public DatabaseFixture()
	{
		using var context = GetContext();

        context.Database.EnsureCreated();
        SeedData(context);
	}

	private static MovieDbContext GetContext()
	{
		return new(new DbContextOptionsBuilder<MovieDbContext>()
			.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
			.Options);
	}

	public void Cleanup()
	{
		using var context = GetContext();

        // deleting indipendent entities, all dependent entities will be automatically deleted
        context.Movies.ExecuteDelete();
        context.Actors.ExecuteDelete();
        context.Users.Where(u => u.Email != "test@moviereservation.com").ExecuteDelete();

        context.SaveChanges();
    }

	private static void SeedData(MovieDbContext context)
	{
        if (context.Users.Any() ||
            context.Roles.Any() ||
            context.Genres.Any() ||
            context.Halls.Any() ||
            context.Seats.Any())
        {
            return;
        }

        foreach (var roleName in Constant<RoleNames>.GetValues())   // Roles
        {
            context.Roles.Add(new Role(roleName!));
        }

        var adminUser = new User("test@moviereservation.com", "test", "test");  // Initial admin
        context.Users.Add(adminUser);

        context.SaveChanges();

        foreach (var role in context.Roles.ToList())
        {
            var userRole = new IdentityUserRole<Guid>() { UserId = adminUser.Id, RoleId = role.Id };
            context.UserRoles.Add(userRole);
        }

        foreach (var genreName in Constant<MovieGenres>.GetValues())    // Genres
        {
            context.Genres.Add(new Genre(genreName!));
        }

        static IEnumerable<Seat> GetSeats(int seatCount)
        {
            for (int i = 1; i <= seatCount; i++)
            {
                yield return new Seat(i);
            }
        }

        var halls = JsonReader.ReadAndDeserialize<Hall>(PathHelper.GetAbsolutePath("Tests/TestData/Halls.json"));   // Halls and seats
        foreach (var hall in halls)
        {
            hall.Seats = GetSeats(hall.SeatCount).ToList();
            context.Halls.Add(hall);
        }

        context.SaveChanges();
    }
}