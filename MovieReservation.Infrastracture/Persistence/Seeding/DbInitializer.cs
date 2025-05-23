using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieReservation.Application.Constants;
using MovieReservation.Application.Options;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Exceptions;

namespace MovieReservation.Infrastracture.Persistence.Seeding;

public static class DbInitializer
{
    public static async Task SeedData(IServiceProvider scopedProvider)
    {
        Random rnd = new();
        var context = scopedProvider.GetRequiredService<MovieDbContext>();
        var userManager = scopedProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<Role>>();
        var admin = scopedProvider.GetRequiredService<IOptions<AdminOptions>>().Value;

        var loggerFactory = scopedProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(DbInitializer));

        context.Database.Migrate();

        if (userManager.Users.Any() ||
            roleManager.Roles.Any() ||
            context.Genres.Any() ||
            context.Halls.Any() ||
            context.Seats.Any())
        {
            return;
        }

        logger.LogInformation("Adding roles to the database");
        foreach (var roleName in Constant<RoleNames>.GetValues())   // Roles
        {
            await roleManager.CreateAsync(new Role(roleName!));
        }

        var adminUser = new User(admin.Email, admin.Name, admin.Name);  // Initial admin
        logger.LogInformation("Adding admin user to the database");
        var createResult = await userManager.CreateAsync(adminUser, admin.Password);
        if (!createResult.Succeeded)
            throw new SeedingException();

        string[] roles = { RoleNames.User, RoleNames.Admin };
        var addRoleResult = await userManager.AddToRolesAsync(adminUser, roles);
        if (!addRoleResult.Succeeded)
            throw new SeedingException();

        logger.LogInformation("Adding genres to the database");
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

        logger.LogInformation("Adding halls and seats to the database");
        for (int i = 1; i <= 10; i++)   // Halls and Seats
        {
            Hall hall = new(i, rnd.Next(100, 300));
            hall.Seats = GetSeats(hall.SeatCount).ToList();

            context.Halls.Add(hall);
        }

        await context.SaveChangesAsync();
    }
}