using FluentResults;
using MovieReservation.Application.DTOs.Showtime;
using MovieReservation.Application.Interfaces.Services.Base;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Application.Interfaces.Services;

public interface IShowtimeService : IServiceBase<Showtime>
{
    Task<Result> Create(CreateShowtimeDto request, CancellationToken token = default);

    Task<Result<ShowtimeDetailsDto>> Get(Guid id, CancellationToken token = default);

    Task<Result> Update(Guid id, UpdateShowtimeDto request, CancellationToken token = default);

    Task<Result<IEnumerable<ShowtimeReservationDto>>> GetReservations(Guid id, CancellationToken token = default);
}