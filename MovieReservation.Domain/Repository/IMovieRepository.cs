using System.Linq.Expressions;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Enums;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Domain.Repository;

public interface IMovieRepository : IRepositoryBase<Movie>
{
	Task<IEnumerable<Movie>> GetBestMatchesByTitleAsync(string title, int count, CancellationToken token = default);

	Task<IEnumerable<Review>> GetReviewsAsync<TKey>(Guid id, Expression<Func<Review, TKey>> sortKeySelector, string sortOrder, int? rating,CancellationToken token = default);

	Task<IEnumerable<IShowtimeInfoQueryResult>> GetShowtimesByDateAsync(Guid id, DateOnly showtimeDate, CancellationToken token = default);

	Task AddRemoveActorAsync(Guid id, Guid actorId, DatabaseOperation dbOperation, CancellationToken token = default);

    Task<IEnumerable<Movie>> SearchByAsync(List<Expression<Func<Movie, bool>>> filters, CancellationToken token = default);
}