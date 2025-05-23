using System.Linq.Expressions;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.DTOs.Actor;
using MovieReservation.Application.DTOs.Movie;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Extensions;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Services.Base;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Application.Services;

public class ActorService : ServiceBase<Actor>, IActorService
{
    private readonly IRepositoryBase<Actor> _actorRepository;
    private readonly IValidator<CreateUpdateActorDto> _createUpdateValidator;

    public ActorService(
        IRepositoryBase<Actor> actorRepository,
        IValidator<CreateUpdateActorDto> createUpdateValidator,
        IDbContext unitOfWork,
        ILogger<ActorService> logger)
        : base(actorRepository, unitOfWork, logger)
    {
        _actorRepository = actorRepository;
        _createUpdateValidator = createUpdateValidator;
    }

    public async Task<Result> Create(CreateUpdateActorDto request, CancellationToken token = default)
    {
        var valResult = _createUpdateValidator.Validate(request);
        if (!valResult.IsValid)
        {
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

        var actor = request.ToActor();

        _logger.LogInformation("Adding new actor to the database");
        _actorRepository.Add(actor);
        await _unitOfWorK.SaveChangesAsync(token);

        return Result.Ok();
    }

    public async Task<Result<ActorDetailsDto>> GetById(Guid id, CancellationToken token = default)
    {
        _logger.LogInformation("Fetching actor with {Id} id", id);
        var actor = await _actorRepository.GetByIdAsync(id, token);
        if (actor is null)
        {
            _logger.LogWarning("Actor with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Actor)));
        }

        return Result.Ok(actor.ToDto());
    }

    public async Task<Result> Update(Guid id, CreateUpdateActorDto request, CancellationToken token = default)
    {
        if (!await _actorRepository.ExistByAsync(a => a.Id, id, token))
        {
            _logger.LogWarning("Actor with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Actor)));
        }

        var valResult = _createUpdateValidator.Validate(request);
        if (!valResult.IsValid)
        {
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

        var actor = request.ToActor(id);

        _logger.LogInformation("Updating actor with {Id} id", id);
        _actorRepository.Update(actor);
        await _unitOfWorK.SaveChangesAsync(token);

        return Result.Ok();
    }

    public async Task<Result<IEnumerable<MovieDetailsDto>>> GetMovies(Guid id, CancellationToken token = default)
    {
        if (!await _actorRepository.ExistByAsync(a => a.Id, id, token))
        {
            _logger.LogWarning("Actor with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Actor)));
        }

        _logger.LogInformation("Fetching movies of actor with {Id} id", id);
        var movies = await _actorRepository.GetRelatedAsync<Movie, object>(id, a => a.Movies, token: token);
        return Result.Ok(movies.ToDtos());
    }
}