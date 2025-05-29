using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MovieReservation.Application.Constants;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Application.Extensions;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Enums;
using MovieReservation.Domain.Repository;
using MovieReservation.Infrastracture.Persistence.Repositories.Base;

namespace MovieReservation.Infrastracture.Persistence.Repositories;

public class MovieRepository : RepositoryBase<Movie>, IMovieRepository
{
    public MovieRepository(MovieDbContext dbContext)
        : base(dbContext)
    {
    }

    public override async Task<IEnumerable<Movie>> GetByAsync<K>(Expression<Func<Movie, K>> propertySelector, K value, CancellationToken token = default)
    {
        var filterExp = propertySelector.GetFilterExpression(value);

        return await _dbContext.Movies
            .Include(m => m.Genres)
            .Where(filterExp)
            .ToListAsync(token);
    }

    public override async Task<IEnumerable<Movie>> GetByAsync<K>(Expression<Func<Movie, K>> propertySelector, K value, CancellationToken token = default, params Expression<Func<Movie, object>>[] navigationSelectors)
    {
        var query = _dbContext.Movies.Include(m => m.Genres).AsQueryable();

        foreach (var navigationSelector in navigationSelectors)
        {
            var navSelector = navigationSelector.CheckAndGetNavigationPropertySelector();
            query = query.Include(navSelector);
        }

        var filterExp = propertySelector.GetFilterExpression(value);
        return await query.Where(filterExp).AsSplitQuery().ToListAsync(token);
    }

    public override async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _dbContext.Movies
            .Include(m => m.Genres)
            .SingleOrDefaultAsync(e => e.Id == id, token);
    }

    public override async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default, params Expression<Func<Movie, object>>[] navigationSelectors)
    {
        var query = _dbContext.Movies.Include(m => m.Genres).AsQueryable();

        foreach (var navigationSelector in navigationSelectors)
        {
            var navSelector = navigationSelector.CheckAndGetNavigationPropertySelector();
            query = query.Include(navSelector);
        }

        return await query.AsSplitQuery().SingleOrDefaultAsync(e => e.Id == id, token);
    }

    public async Task<IEnumerable<Movie>> GetBestMatchesByTitleAsync(string title, int count, CancellationToken token = default)
    {
        return await _dbContext.Movies
            .AsNoTracking()
            .Include(m => m.Genres)
            .Where(m => MovieDbContext.Levenshtein(m.Title, title) <= 4)    // get the titles with a levenstein distance <= 4 from "title"
            .Take(count).ToListAsync(token);                                // return the "count" best matches
    }

    public async Task<IEnumerable<Review>> GetReviewsAsync<TKey>(Guid id, Expression<Func<Review, TKey>> sortKeySelector, string sortOrder, int? rating, CancellationToken token = default)
    {
        var keySelector = sortKeySelector.CheckAndGetSortKeySelector();
        bool isDescending = sortOrder == SortingOrders.Desc;

        return rating is null ?
                (isDescending ? await _dbContext.Movies.AsNoTracking().Where(m => m.Id == id).SelectMany(m => m.Reviews).Include(r => r.Reactions).OrderByDescending(keySelector).ToListAsync(token)
                : await _dbContext.Movies.AsNoTracking().Where(m => m.Id == id).SelectMany(m => m.Reviews).Include(r => r.Reactions).OrderBy(keySelector).ToListAsync(token))
            :
                (isDescending ? await _dbContext.Movies.AsNoTracking().Where(m => m.Id == id).SelectMany(m => m.Reviews).Include(r => r.Reactions).Where(r => r.Rating == rating).OrderByDescending(keySelector).ToListAsync(token)
                : await _dbContext.Movies.AsNoTracking().Where(m => m.Id == id).SelectMany(m => m.Reviews).Include(r => r.Reactions).Where(r => r.Rating == rating).OrderBy(keySelector).ToListAsync(token));

    }
    
    public async Task<IEnumerable<IShowtimeInfoQueryResult>> GetShowtimesByDateAsync(Guid id, DateOnly showtimeDate, CancellationToken token = default)
    {
        return await _dbContext.Movies
            .Where(m => m.Id == id)
            .SelectMany(
                m => m.Showtimes.Where(
                    s => s.Timestamp > DateTime.UtcNow 
                    && DateOnly.FromDateTime(s.Timestamp) == showtimeDate),
                (m, s) => new ShowtimeInfoDto { Timestamp = s.Timestamp })
            .OrderBy(s => s.Timestamp)
            .ToListAsync(token);
    }

    public async Task AddRemoveActorAsync(Guid id, Guid actorId, DatabaseOperation dbOperation, CancellationToken token = default)
    {
        switch (dbOperation)
        {
            case DatabaseOperation.Add:
                await _dbContext.Database.ExecuteSqlAsync($"INSERT INTO MovieActor (ActorId, MovieId) VALUES ({actorId}, {id})", token);
                break;
            case DatabaseOperation.Remove:
                await _dbContext.Database.ExecuteSqlAsync($"DELETE FROM MovieActor WHERE ActorId = {actorId} AND MovieId = {id}", token);
                break;
        }
    }

    public async Task<IEnumerable<Movie>> SearchByAsync(List<Expression<Func<Movie, bool>>> filters, CancellationToken token = default)
    {
        var query = _dbContext.Movies.AsNoTracking().Include(m => m.Genres).AsQueryable();
        foreach (var filter in filters)
        {
            query = query.Where(filter);
        }

        return await query.ToListAsync(token);
    }
}