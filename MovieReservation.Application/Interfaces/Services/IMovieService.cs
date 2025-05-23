using System.Linq.Expressions;
using FluentResults;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.Interfaces.Services.Base;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Enums;

namespace MovieReservation.Application.Interfaces.Services;

public interface IMovieService : IServiceBase<Movie>
{
    Task<Result> Create(CreateUpdateMovieDto request, CancellationToken token = default);

    Task<IEnumerable<MovieDetailsDto>> GetBy<K>(Expression<Func<Movie, K>> propertySelector, K values, CancellationToken token = default);

    Task<Result<MovieDetailsDto>> GetById(Guid id, CancellationToken token = default);

    Task<IEnumerable<MovieDetailsDto>> GetBestMatchesByTitle(string title, int movieCount, CancellationToken token = default);

    Task<Result> Update(Guid id, CreateUpdateMovieDto request, CancellationToken token = default);

    Task<Result<IEnumerable<ReviewDetailsDto>>> GetReviews(Guid id, string sort, string sortOrder, int? rating, CancellationToken token = default);

    Task<Result<IEnumerable<ActorDetailsDto>>> GetActors(Guid id, CancellationToken token = default);

    Task<Result<IEnumerable<IShowtimeInfoQueryResult>>> GetShowtimes(Guid id, DateOnly? showtimeDate, CancellationToken token = default);

    Task<Result> AddReview(Guid id, Guid userId, CreateUpdateReviewDto request, CancellationToken token = default);

    Task<Result> AddRemoveActor(Guid id, Guid actorId, DatabaseOperation dbOperation, CancellationToken token = default);
}