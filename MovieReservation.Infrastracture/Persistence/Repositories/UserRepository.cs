using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Application.DTOs.Reservation;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken token = default)
    {
        return !await _userManager.Users.AnyAsync(u => u.Email == email, token);
    }

    public async Task<IEnumerable<IReservationDetailsQueryResult>> GetReservationsAsync(Guid id, CancellationToken token = default)
    {
        return await _userManager.Users
            .Where(u => u.Id == id)
            .SelectMany(u => u.Reservations.Select(r => new ReservationDetailsDto
            {
                Code = r.Code,
                MovieTitle = r.Showtime.Movie.Title,
                ShowtimeStart = r.Showtime.Timestamp,
                HallNumber = r.Showtime.Hall.Number,
                SeatsNumber = r.Seats.Select(s => s.Number)
            }))
            .ToListAsync(token);
    }
}