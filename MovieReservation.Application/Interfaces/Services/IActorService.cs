using System.Linq.Expressions;
using FluentResults;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.Interfaces.Services.Base;
using MovieReservation.Domain.Entities;

namespace MovieReservation.Application.Interfaces.Services;

public interface IActorService : IServiceBase<Actor>
{
    Task<Result> Create(CreateUpdateActorDto request, CancellationToken token = default);

    Task<Result<ActorDetailsDto>> GetById(Guid id, CancellationToken token = default);
    
    Task<Result> Update(Guid id, CreateUpdateActorDto request, CancellationToken token = default);

    Task<Result<IEnumerable<MovieDetailsDto>>> GetMovies(Guid id, CancellationToken token = default);
}