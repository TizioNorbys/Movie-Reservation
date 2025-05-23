using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Domain.Repository;

public interface IReactionRepository : IRepositoryBase<Reaction>
{
    Task<Reaction?> GetByReviewAndUser(Guid reviewId, Guid userId, CancellationToken token = default);

    Task<bool> HasAlreadyReacted(Guid reviewId, Guid userId, CancellationToken token = default);
}