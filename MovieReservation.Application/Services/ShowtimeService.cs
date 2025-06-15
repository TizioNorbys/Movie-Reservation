using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Extensions;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Services.Base;
using MovieReservation.Domain.Abstractions.QueryResults;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Application.Services;

public class ShowtimeService : ServiceBase<Showtime>, IShowtimeService
{
	private readonly IShowtimeRepository _showtimeRepository;
	private readonly IMovieRepository _movieRepository;
	private readonly IRepositoryBase<Hall> _hallRepository;
	private readonly IRepositoryBase<Reservation> _reservationRepository;
	private readonly IValidator<ICreateUpdateShowtimeDto> _createUpdateValidator;

	public ShowtimeService(
        IShowtimeRepository showtimeRepository,
        IMovieRepository movieRepository,
        IRepositoryBase<Hall> hallRepository,
        IRepositoryBase<Reservation> reservationRepository,
        IDbContext unitOfWork,
        IValidator<ICreateUpdateShowtimeDto> createUpdateValidator,
		ILogger<ShowtimeService> logger)
		: base(showtimeRepository, unitOfWork, logger)
	{
		_showtimeRepository = showtimeRepository;
        _movieRepository = movieRepository;
		_hallRepository = hallRepository;
		_reservationRepository = reservationRepository;
		_createUpdateValidator = createUpdateValidator;
	}

	public async Task<Result> Create(CreateShowtimeDto request, CancellationToken token = default)
	{
		if (!await _hallRepository.ExistByAsync(h => h.Id, request.HallId, token))
		{
			_logger.LogWarning("Hall with {Id} id not found", request.HallId);
			return Result.Fail(AppErrors.NotFound(typeof(Hall)));
		}

		if (!await _movieRepository.ExistByAsync(m => m.Id, request.MovieId, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", request.MovieId);
			return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }

		var valResult = _createUpdateValidator.Validate(request);
		if (!valResult.IsValid)
		{
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
			return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

		var showtime = request.ToShowtime();

		_logger.LogInformation("Adding new showtime to the database");
		_showtimeRepository.Add(showtime);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
	}

	public async Task<Result<ShowtimeDetailsDto>> Get(Guid id, CancellationToken token = default)
	{
		_logger.LogInformation("Fetching showtime with {Id} id", id);
		var showtime = await _showtimeRepository.GetByIdAsync(id, token, s => s.Hall);
		if (showtime is null)
		{
            _logger.LogWarning("Showtime with {Id} id not found", id);
			return Result.Fail(AppErrors.NotFound(typeof(Showtime)));
        }

		var showtimeSeatsStatus = await _showtimeRepository.GetShowtimeSeatsStatusAsync(id, token);
		return Result.Ok(showtime.ToDto(showtimeSeatsStatus!));
	}

    public async Task<Result> Update(Guid id, UpdateShowtimeDto request, CancellationToken token = default)
	{
		var showtime = await _showtimeRepository.GetByIdAsync(id, token);
		if (showtime is null)
			return Result.Fail(AppErrors.NotFound(typeof(Showtime)));

        if (!await _movieRepository.ExistByAsync(m => m.Id, request.MovieId, token))
		{
            _logger.LogWarning("Movie with {Id} id not found", request.MovieId);
            return Result.Fail(AppErrors.NotFound(typeof(Movie)));
        }

        var valResult = _createUpdateValidator.Validate(request);
        if (!valResult.IsValid)
		{
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

		_showtimeRepository.SetValues(showtime, request);
		_logger.LogInformation("Updating showtime with {Id} id", id);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
    }

    public async Task<Result<IEnumerable<IShowtimeReservationQueryResult>>> GetReservations(Guid id, CancellationToken token = default)
    {
		if (!await _showtimeRepository.ExistByAsync(s => s.Id, id, token))
		{
            _logger.LogWarning("Showtime with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Showtime)));
		}

		_logger.LogInformation("Fetching reservations of showtime with {Id} id", id);
		var showtimeReservations = await _showtimeRepository.GetShowtimeReservations(id, token);
		return Result.Ok(showtimeReservations);
    }
}