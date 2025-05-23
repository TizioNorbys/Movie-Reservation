using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Domain.Repository;

public interface IShowtimeRepository : IRepositoryBase<Showtime>
{
    Task<IShowtimeSeatsQueryResult?> GetShowtimeSeatsStatusAsync(Guid id, CancellationToken token = default);
}