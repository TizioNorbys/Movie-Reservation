using MovieReservation.Domain.Abstractions.QueryResults;

namespace MovieReservation.Domain.Repository;

public interface IUserRepository
{
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken token = default);

    Task<IEnumerable<IReservationDetailsQueryResult>> GetReservationsAsync(Guid id, CancellationToken token = default);
}