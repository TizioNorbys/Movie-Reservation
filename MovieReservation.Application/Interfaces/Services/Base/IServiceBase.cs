using FluentResults;
using MovieReservation.Domain.Entities.Base;

namespace MovieReservation.Application.Interfaces.Services.Base;

public interface IServiceBase<T>
    where T : IEntityBase
{
    Task<Result> Delete(Guid id, CancellationToken token = default);
}