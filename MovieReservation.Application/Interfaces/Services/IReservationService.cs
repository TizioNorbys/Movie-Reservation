using FluentResults;
using MovieReservation.Application.DTOs.Reservation;
using MovieReservation.Application.Interfaces.Services.Base;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Application.Interfaces.Services;

public interface IReservationService : IServiceBase<Reservation>
{
    Task<Result> Create(Guid userId, CreateReservationDto request, CancellationToken token = default);

    Task<Result> Delete(Guid id, Guid userId, CancellationToken token = default);
}