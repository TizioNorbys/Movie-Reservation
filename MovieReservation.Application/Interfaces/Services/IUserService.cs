using MovieReservation.Domain.Abstractions.QueryResults;

namespace MovieReservation.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<IReservationDetailsQueryResult>> GetReservations(Guid userId, CancellationToken token = default);
}