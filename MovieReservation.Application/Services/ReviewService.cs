using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.DTOs.Review;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Extensions;
using MovieReservation.Application.Interfaces.Data;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Application.Services.Base;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Enums;
using MovieReservation.Domain.Repository;
using MovieReservation.Domain.Repository.Base;

namespace MovieReservation.Application.Services;

public class ReviewService : ServiceBase<Review>, IReviewService
{
	private readonly IReactionRepository _reactionRepository;
    private readonly IRepositoryBase<Review> _reviewRepository;
	private readonly IValidator<CreateUpdateReviewDto> _createUpdateValidator;

	public ReviewService(
        IReactionRepository reactionRepository,
		IRepositoryBase<Review> reviewRepository,
		IDbContext unitOfWork,
        IValidator<CreateUpdateReviewDto> createUpdateValidator,
		ILogger<ReviewService> logger)
		: base(reviewRepository, unitOfWork, logger)
	{
		_reactionRepository = reactionRepository;
		_reviewRepository = reviewRepository;
		_createUpdateValidator = createUpdateValidator;
	}

	public async Task<Result<ReviewDetailsDto>> Get(Guid id, CancellationToken token = default)
	{
		_logger.LogInformation("Fetching review with {Id} id", id);
		var review = await _reviewRepository.GetByIdAsync(id, token, r => r.Reactions);
        if (review is null)
		{
			_logger.LogWarning("Review with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(Review)));
		}

        return Result.Ok(review.ToDto());
    }

	public async Task<Result> Update(Guid id, Guid userId, CreateUpdateReviewDto request, CancellationToken token = default)
	{
        _logger.LogInformation("Fetching review with {Id} id", id);
        var review = await _reviewRepository.GetByIdAsync(id, token);
		if (review is null)
		{
            _logger.LogWarning("Review with {Id} id not found", id);
			return Result.Fail(AppErrors.NotFound(typeof(Review)));
        }

		if (review.UserId != userId)
			return Result.Fail(AppErrors.Forbid);

		var valResult = _createUpdateValidator.Validate(request);
		if (!valResult.IsValid)
		{
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
			return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

		_reviewRepository.SetValues(review, request);
		_logger.LogInformation("Updating review with {Id} id", id);
        await _unitOfWorK.SaveChangesAsync(token);

        return Result.Ok();
	}

    public async Task<Result> Delete(Guid id, Guid userId, CancellationToken token = default)
    {
		_logger.LogInformation("Fetching review with {Id} id", id);
		var review = await _reviewRepository.GetByIdAsync(id, token);
		if (review is null)
		{
            _logger.LogWarning("Review with {Id} id not found", id);
			return Result.Fail(AppErrors.NotFound(typeof(Review)));
        }

		if (review.UserId != userId)
			return Result.Fail(AppErrors.Forbid);

        _logger.LogInformation("Deleting review with {Id} id", id);
        _reviewRepository.Delete(review);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
    }

	public async Task<Result> AddReaction(Guid reviewId, Guid userId, ReactionType reactionType, CancellationToken token = default)
	{
		if (!await _reviewRepository.ExistByAsync(r => r.Id, reviewId, token))
		{
            _logger.LogWarning("Review with {Id} id not found", reviewId);
			return Result.Fail(AppErrors.NotFound(typeof(Review)));
        }

		if (await _reactionRepository.HasAlreadyReacted(reviewId, userId, token))
			return Result.Fail(AppErrors.AlreadyReacted);

		bool type = reactionType == ReactionType.Like;
		Reaction reaction = new(type, DateTime.UtcNow, reviewId, userId);

		_logger.LogInformation("Adding reaction to review with {Id} id", reviewId);
		_reactionRepository.Add(reaction);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
	}

	public async Task<Result> RemoveReaction(Guid reviewId, Guid userId, CancellationToken token = default)
	{
        if (!await _reviewRepository.ExistByAsync(r => r.Id, reviewId, token))
		{
            _logger.LogWarning("Review with {Id} id not found", reviewId);
            return Result.Fail(AppErrors.NotFound(typeof(Review)));
        }

        var reaction = await _reactionRepository.GetByReviewAndUser(reviewId, userId, token);
		if (reaction is null)
		{
			_logger.LogWarning("Reaction of review with {ReviewId} id by user with {UserId} id not found", reviewId, userId);
			return Result.Fail(AppErrors.NotFound(typeof(Reaction)));
		}

        _logger.LogInformation("Deleting reaction from review with {Id} id", reviewId);
        _reactionRepository.Delete(reaction);
		await _unitOfWorK.SaveChangesAsync(token);

		return Result.Ok();
	}
}