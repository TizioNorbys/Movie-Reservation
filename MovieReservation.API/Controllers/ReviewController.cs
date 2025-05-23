using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.API.Requests.Review;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Errors.Authorization;
using MovieReservation.Application.Errors.Review;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Enums;

namespace MovieReservation.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
	private readonly IReviewService _reviewService;

	public ReviewController(IReviewService reviewService)
	{
		_reviewService = reviewService;
	}

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken token = default)
    {
        var result = await _reviewService.Get(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreateUpdateReviewRequest request, CancellationToken token = default)
    {
        var result = await _reviewService.Update(id, User.GetUserId(), request.ToDto(), token);
        if (result.IsFailed)
        {
            return result.GetFirstError() switch
            {
                NotFoundError notFound => NotFound(new { notFound.Message }),
                ForbidError => Forbid(),
                ValidationError valError => ValidationProblem(ModelState.AddValidationErrors(valError.Metadata))
            };
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token = default)
    {
        var result = await _reviewService.Delete(id, User.GetUserId(), token);
        if (result.IsFailed)
        {
            return result.GetFirstError() switch
            {
                NotFoundError notFound => NotFound(new { notFound.Message }),
                ForbidError => Forbid()
            };
        }

        return NoContent();
    }

    [HttpPost("{id}/likes")]
    public async Task<IActionResult> AddLike(Guid id, CancellationToken token = default)
    {
        var result = await _reviewService.AddReaction(id, User.GetUserId(), ReactionType.Like, token);
        if (result.IsFailed)
        {
            return result.GetFirstError() switch
            {
                NotFoundError notFound => NotFound(new { notFound.Message }),
                AlreadyReactedError err => Conflict(new { err.Message })
            };
        }

        return NoContent();
    }

    [HttpPost("{id}/dislikes")]
    public async Task<IActionResult> AddDislike(Guid id, CancellationToken token = default)
    {
        var result = await _reviewService.AddReaction(id, User.GetUserId(), ReactionType.Dislike, token);
        if (result.IsFailed)
        {
            return result.GetFirstError() switch
            {
                NotFoundError notFound => NotFound(new { notFound.Message }),
                AlreadyReactedError err => Conflict(new { err.Message })
            };
        }

        return NoContent();
    }

    [HttpDelete("{id}/reactions")]
    public async Task<IActionResult> RemoveReaction(Guid id, CancellationToken token = default)
    {
        var result = await _reviewService.RemoveReaction(id, User.GetUserId(), token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return NoContent();
    }
}