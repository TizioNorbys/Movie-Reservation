using Microsoft.EntityFrameworkCore;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class ReactionRepository : RepositoryBase<Reaction>, IReactionRepository
{
    public ReactionRepository(MovieDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Reaction?> GetByReviewAndUser(Guid reviewId, Guid userId, CancellationToken token = default)
    {
        return await _dbContext.Reactions.SingleOrDefaultAsync(r => r.ReviewId == reviewId && r.UserId == userId, token);
    }

    public async Task<bool> HasAlreadyReacted(Guid reviewId, Guid userId, CancellationToken token = default)
    {
        return await _dbContext.Reactions.AnyAsync(r => r.ReviewId == reviewId && r.UserId == userId, token);
    }
}