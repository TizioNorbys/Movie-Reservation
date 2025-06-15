using System.Linq.Expressions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.Constants;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Extensions;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Services.Base;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Enums;
using MovieReservation.Domain.Repository;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Application.Services;

public class MovieService : ServiceBase<Movie>, IMovieService
{
	private readonly IMovieRepository _movieRepository;
	private readonly IRepositoryBase<Review> _reviewRepository;
	private readonly IRepositoryBase<Actor> _actorRepository;
	private readonly IRepositoryBase<Genre> _genreRepository;
	private readonly IValidator<CreateUpdateMovieDto> _createUpdateValidator;
	private readonly IValidator<CreateUpdateReviewDto> _createReviewValidator;

	public MovieService(
		IMovieRepository movieRepository,
        IRepositoryBase<Review> reviewRepository,
        IRepositoryBase<Actor> actorRepository,
        IRepositoryBase<Genre> genreRepository,
		IDbContext unitOfWork,
		IValidator<CreateUpdateMovieDto> createUpdateValidator,
        IValidator<CreateUpdateReviewDto> createReviewValidator,
        ILogger<MovieService> logger)
		: base(movieRepository, unitOfWork, logger)
	{
		_movieRepository = movieRepository;
		_reviewRepository = reviewRepository;
		_actorRepository = actorRepository;
		_genreRepository = genreRepository;
		_createUpdateValidator = createUpdateValidator;
		_createReviewValidator = createReviewValidator;
	}

	public async Task<Result> Create(CreateUpdateMovieDto request, CancellationToken token = default)
	{
		var valResult = _createUpdateValidator.Validate(request);
        if (!valResult.IsValid)
		{
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
        }
			
		var movie = request.ToMovie();
		var genres = await _genreRepository.GetByMultipleValuesAsync(g => g.Name, request.Genres, token);
		movie.Genres = genres.ToList();

		_logger.LogInformation("Adding new movie to the database");
		_movieRepository.Add(movie);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
	}

	public async Task<IEnumerable<MovieDetailsDto>> GetBy<K>(Expression<Func<Movie, K>> propertySelector, K value, CancellationToken token = default)
	{
		_logger.LogInformation("Fetching movies");
		var movies = await _movieRepository.GetByAsync(propertySelector, value, token);

		return movies.ToDtos();
	}

	public async Task<Result<MovieDetailsDto>> GetById(Guid id, CancellationToken token = default)
	{
		_logger.LogInformation("Fetching movie with {Id} id", id);
		var movie = await _movieRepository.GetByIdAsync(id, token);
		if (movie is null)
		{
            _logger.LogWarning("Movie with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }
		return Result.Ok(movie.ToDto());
	}

	public async Task<IEnumerable<MovieDetailsDto>> GetBestMatchesByTitle(string title, int movieCount, CancellationToken token = default)
	{
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

		_logger.LogInformation("Fetching movies by {Title} title using levenshtein distance", title);
		var movies = await _movieRepository.GetBestMatchesByTitleAsync(title, movieCount, token);

        return movies.ToDtos();
    }

	public async Task<Result> Update(Guid id, CreateUpdateMovieDto request, CancellationToken token = default)
	{
        var movie = await _movieRepository.GetByIdAsync(id, token);
		if (movie is null)
			return Result.Fail(AppErrors.NotFound(typeof(Movie)));

        var valResult = _createUpdateValidator.Validate(request);
		if (!valResult.IsValid)
		{
			_logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

		_movieRepository.SetValues(movie, request);
		var genres = await _genreRepository.GetByMultipleValuesAsync(g => g.Name, request.Genres, token);
		movie.Genres = genres.ToList();

		_logger.LogInformation("Updating movie with {Id} id", id);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
	}

	public async Task<Result<IEnumerable<ActorDetailsDto>>> GetActors(Guid id, CancellationToken token = default)
	{
		if (!await _movieRepository.ExistByAsync(m => m.Id, id, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }
			
        _logger.LogInformation("Fetching actors of movie with {Id} id", id);
        var actors = await _movieRepository.GetRelatedAsync<Actor, object>(id, m => m.Actors, token: token);

		return Result.Ok(actors.ToDtos());
	}

	public async Task<Result<IEnumerable<ReviewDetailsDto>>> GetReviews(Guid id, string sort, string sortOrder, int? rating, CancellationToken token = default)
	{
		if (!await _movieRepository.ExistByAsync(m => m.Id, id, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }

		if (!Constant<SortingOrders>.IsDefined(sortOrder))
			sortOrder = SortingOrders.Desc;

		_logger.LogInformation("Fetching reviews of movie with {Id} id", id);
		var sortKeySelector = GetReviewSortKeySelector(sort);
		var reviews = await _movieRepository.GetReviewsAsync(id, sortKeySelector, sortOrder, rating, token);

		return Result.Ok(reviews.ToDtos());
	}

	private static Expression<Func<Review, object>> GetReviewSortKeySelector(string sort)
	{
		return sort switch
		{
			ReviewSortValues.Rating => r => r.Rating,
			ReviewSortValues.Reactions => r => r.Reactions.Count,
			ReviewSortValues.Date => r => r.Timestamp,
			_ => r => r.Rating
		};
    }

	public async Task<Result<IEnumerable<IShowtimeInfoQueryResult>>> GetShowtimes(Guid id, DateOnly? showtimeDate, CancellationToken token = default)
	{
		if (!await _movieRepository.ExistByAsync(m => m.Id, id, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }
			
		if (showtimeDate is null)
		{
			_logger.LogInformation("Fetching showtimes of movie with {Id} id", id);
			var showtimes = await _movieRepository.GetRelatedAsync(id, m => m.Showtimes, s => s.Timestamp, token);
			showtimes = showtimes.Where(s => s.Timestamp >= DateTime.UtcNow);

			return Result.Ok(showtimes.ToDtos() as IEnumerable<IShowtimeInfoQueryResult>);
		}

        _logger.LogInformation("Fetching showtimes of movie with {Id} id", id);
        var dateShowtimes = await _movieRepository.GetShowtimesByDateAsync(id, showtimeDate.Value, token);

		return Result.Ok(dateShowtimes);
	}

    public async Task<Result> AddReview(Guid id, Guid userId, CreateUpdateReviewDto request, CancellationToken token = default)
    {
        if (!await _movieRepository.ExistByAsync(m => m.Id, id, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }

		var valResult = _createReviewValidator.Validate(request);
        if (!valResult.IsValid)
		{
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
		}

		var review = request.ToReview(id, userId);

		_logger.LogInformation("Adding review with {ReviewId} id to movie with {MovieId} id", review.Id, id);
		_reviewRepository.Add(review);
        await _unitOfWorK.SaveChangesAsync(token);

        return Result.Ok();
    }

	public async Task<Result> AddRemoveActor(Guid id, Guid actorId, DatabaseOperation dbOperation, CancellationToken token = default)
	{
		if (!await _movieRepository.ExistByAsync(m => m.Id, id, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
		}

		if (!await _actorRepository.ExistByAsync(a => a.Id, actorId, token))
		{
            _logger.LogWarning("Actor with {Id} id not found", actorId);
            return Result.Fail(AppErrors.NotFound(typeof(Actor)));
		}

		_logger.LogInformation("Adding/removing actor with {ActorId} id to/from movie with {MovieId} id", actorId, id);
		await _movieRepository.AddRemoveActorAsync(id, actorId, dbOperation, token);

		return Result.Ok();
	}
}