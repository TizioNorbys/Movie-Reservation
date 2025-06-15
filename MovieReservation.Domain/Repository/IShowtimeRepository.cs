using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Domain.Repository;

public interface IShowtimeRepository : IRepositoryBase<Showtime>
{
    Task<IEnumerable<IShowtimeReservationQueryResult>> GetShowtimeReservations(Guid id, CancellationToken token = default);

    Task<IShowtimeSeatsQueryResult?> GetShowtimeSeatsStatusAsync(Guid id, CancellationToken token = default);
}