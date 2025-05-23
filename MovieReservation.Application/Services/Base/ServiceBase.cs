using FluentResults;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Interfaces.Services.Base;
using MovieReservation.Domain.Entities.Base;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Application.Services.Base;

public abstract class ServiceBase<T> : IServiceBase<T>
    where T : class, IEntityBase, new()
{
	private readonly IRepositoryBase<T> _repository;
	protected readonly IDbContext _unitOfWorK;
	protected readonly ILogger _logger;

    protected ServiceBase(IRepositoryBase<T> repository, IDbContext unitOfWorK, ILogger logger)
	{
		_repository = repository;
		_unitOfWorK = unitOfWorK;
		_logger = logger;
	}

	public virtual async Task<Result> Delete(Guid id, CancellationToken token = default)
	{
		if (!await _repository.ExistByAsync(e => e.Id, id, token))
		{
            _logger.LogWarning("Entity of {entityType} type with {id} id not found", typeof(T).Name, id);
			return Result.Fail(AppErrors.NotFound(typeof(T)));
        }

		_logger.LogInformation("Deleting {entityType} entity witch {id} id", typeof(T).Name, id);
		T entity = new() { Id = id };
		_repository.Delete(entity);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
	}
}