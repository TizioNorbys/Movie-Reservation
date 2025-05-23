using FluentResults;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.DTOs.Reservation;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Services.Base;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Application.Services;

public class ReservationService : ServiceBase<Reservation>, IReservationService
{
	private static readonly SemaphoreSlim semaphore = new(1, 1);
	private readonly IShowtimeRepository _showtimeRepository;
	private readonly IRepositoryBase<Reservation> _reservationRepository;
	private readonly IRepositoryBase<Seat> _seatRepository;

    public ReservationService(
        IShowtimeRepository showtimeRepository,
		IRepositoryBase<Reservation> reservationRepository,
        IRepositoryBase<Seat> seatRepository,
        IDbContext unitOfWork,
		ILogger<ReservationService> logger)
		: base(reservationRepository, unitOfWork, logger)
	{
		_showtimeRepository = showtimeRepository;
		_reservationRepository = reservationRepository;
		_seatRepository = seatRepository;
	}

    public async Task<Result> Create(Guid userId, CreateReservationDto request, CancellationToken token = default)
	{
		_logger.LogInformation("Fetching showtime with {Id} id", request.ShowtimeId);
        var showtime = await _showtimeRepository.GetByIdAsync(request.ShowtimeId, token);
		if (showtime is null)
		{
			_logger.LogWarning("Showtime with {Id} id not found", request.ShowtimeId);
			return Result.Fail(AppErrors.NotFound(typeof(Showtime)));
		}

		if (!request.SeatsIds.Any())
			return Result.Fail(AppErrors.NoSeats);

		if (request.SeatsIds.Count() > 10)
			return Result.Fail(AppErrors.TooManySeats);

		Reservation reservation = new("abcd1234", userId, request.ShowtimeId);

		await semaphore.WaitAsync(token);
		try
		{
            foreach (var seatId in request.SeatsIds.Distinct())
            {
                _logger.LogInformation("Fetching seat with {Id} id", seatId);
                var seat = await _seatRepository.GetByIdAsync(seatId, token, s => s.SeatReservations);
                if (seat is null)
                {
                    _logger.LogWarning("Seat with {Id} id no found", seatId);
                    return Result.Fail(AppErrors.NotFound(typeof(Seat)));
                }

                if (seat.HallId != showtime.HallId)
                    return Result.Fail(AppErrors.WrongHall);

                if (seat.SeatReservations.Any(sr => sr.ShowtimeId == request.ShowtimeId)) // check if the seat is already reserved
                    return Result.Fail(AppErrors.AlreadyReserved(seat.Number));

                reservation.SeatReservations.Add(new SeatReservation
                {
                    Seat = seat,
                    Showtime = showtime
                });
            }

            _logger.LogInformation("Adding new reservation to the database");
            _reservationRepository.Add(reservation);
            await _unitOfWorK.SaveChangesAsync(token);
        }
		finally
		{
			semaphore.Release();
		}
		
        return Result.Ok();
	}

    public async Task<Result> Delete(Guid id, Guid userId, CancellationToken token = default)
    {
		_logger.LogInformation("Fetching reservation with {Id} id", id);
		var reservation = await _reservationRepository.GetByIdAsync(id, token, r => r.Showtime);
		if (reservation is null)
		{
			_logger.LogWarning("Reservation with {Id} id not found", id);
			return Result.Fail(AppErrors.NotFound(typeof(Reservation)));
		}

		if (reservation.UserId != userId)
			return Result.Fail(AppErrors.Forbid);

		if (DateTime.UtcNow > reservation.Showtime.Timestamp - TimeSpan.FromHours(12))
			return Result.Fail(AppErrors.TooLateToCancel);

		_logger.LogInformation("Deleting reservation with {Id} id", id);
		_reservationRepository.Delete(reservation);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
    }
}