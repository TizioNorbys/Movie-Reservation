using Microsoft.EntityFrameworkCore;
using MovieReservation.Application.DTOs.Seat;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class ShowtimeRepository : RepositoryBase<Showtime>, IShowtimeRepository
{
	public ShowtimeRepository(MovieDbContext dbContext)
		: base(dbContext)
	{
	}

    public async Task<IEnumerable<IShowtimeReservationQueryResult>> GetShowtimeReservations(Guid id, CancellationToken token)
    {
        return await _dbContext.Showtimes
            .Where(s => s.Id == id)
            .SelectMany(s => s.Reservations.Select(r => new ShowtimeReservationDto
            {
                Code = r.Code,
                UserName = r.User.UserName!,
                SeatsReserved = r.Seats.Select(s => s.Number)
            }))
            .ToListAsync(token);
    }

    public async Task<IShowtimeSeatsQueryResult?> GetShowtimeSeatsStatusAsync(Guid id, CancellationToken token = default)
	{
        return await _dbContext.Showtimes
                .Where(sh => sh.Id == id)
                .Select(sh => new ShowtimeSeatsStatusDto
                {
                    AvailableSeats = sh.Hall.Seats.Count(se => !sh.SeatReservations.Any(sr => sr.SeatId == se.Id)),
                    SeatsStatus = sh.Hall.Seats.Select(se => new SeatStatusDto
                    {
                        Number = se.Number,
                        IsReserved = sh.SeatReservations.Any(sr => sr.SeatId == se.Id)
                    })
                })
                .SingleOrDefaultAsync(token);
    }
}