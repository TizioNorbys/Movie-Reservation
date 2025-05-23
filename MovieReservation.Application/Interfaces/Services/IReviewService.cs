using System.Linq.Expressions;
using FluentResults;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.Interfaces.Services.Base;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Enums;

namespace MovieReservation.Application.Interfaces.Services;

public interface IReviewService : IServiceBase<Review>
{
    Task<Result<ReviewDetailsDto>> Get(Guid id, CancellationToken token = default);

    Task<Result> Update(Guid id, Guid userId, CreateUpdateReviewDto request, CancellationToken token = default);

    Task<Result> Delete(Guid id, Guid userId, CancellationToken token = default);

    Task<Result> AddReaction(Guid reviewId, Guid userId, ReactionType reactionType, CancellationToken token = default);

    Task<Result> RemoveReaction(Guid reviewId, Guid userId, CancellationToken token = default);
}