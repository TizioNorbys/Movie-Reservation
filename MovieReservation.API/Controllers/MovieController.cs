using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.API.Requests.Movie;
using MovieReservation.API.Requests.Review;
using MovieReservation.Application.Constants;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Errors.Movie;
using MovieReservation.Application.Extensions;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Enums;

namespace MovieReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
	private readonly IMovieService _movieService;

	public MovieController(IMovieService movieService)
	{
		_movieService = movieService;
	}

	[Authorize(Roles = RoleNames.Admin)]
	[HttpPost]
	public async Task<IActionResult> Create(CreateUpdateMovieRequest request, CancellationToken token = default)
	{
		var result = await _movieService.Create(request.ToDto(), token);
		if (result.IsFailed)
		{
			var error = result.GetFirstError();
			return ValidationProblem(ModelState.AddValidationErrors(error.Metadata));
		}

		return Created();
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Get(Guid id, CancellationToken token = default)
	{
		var result = await _movieService.GetById(id, token);
		if (result.IsFailed)
			return NotFound(new { result.GetFirstError().Message });

		return Ok(result.Value);
	}

	[HttpGet("{id}/actors")]
	public async Task<IActionResult> GetActors(Guid id, CancellationToken token = default)
	{
		var result = await _movieService.GetActors(id, token);
		if (result.IsFailed)
			return NotFound(new { result.GetFirstError().Message });

		return Ok(result.Value);
	}

	[HttpGet("{id}/reviews")]
	public async Task<IActionResult> GetReviews(Guid id, [FromQuery] string sort = ReviewSortValues.Rating, [FromQuery] string order = SortingOrders.Desc, [FromQuery] int? rating = null, CancellationToken token = default)
	{
		var result = await _movieService.GetReviews(id, sort, order, rating, token);
		if (result.IsFailed)
			return NotFound(new { result.GetFirstError().Message });

        return result.Value.Any() ? Ok(result.Value) : NotFound(new { Message = "No result" });
    }

	[HttpGet("{id}/showtimes")]
	public async Task<IActionResult> GetShowtimes(Guid id, [FromQuery] DateOnly? date = null, CancellationToken token = default)
	{
		if (date is not null && date < DateOnly.FromDateTime(DateTime.UtcNow))
			return BadRequest();

		var result = await _movieService.GetShowtimes(id, date, token);
		if (result.IsFailed)
			return NotFound(new { result.GetFirstError().Message });

		return result.Value.Any() ? Ok(result.Value) : NotFound(new { Message = "No result"});
	}

	[Authorize(Roles = RoleNames.Admin)]
	[HttpPut("{id}")]
	public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateUpdateMovieRequest request, CancellationToken token = default)
	{
		var result = await _movieService.Update(id, request.ToDto(), token);
		if (result.IsFailed)
		{
			return result.GetFirstError() switch
			{
				NotFoundError notFound => NotFound(new { notFound.Message }),
				ValidationError valErr => ValidationProblem(ModelState.AddValidationErrors(valErr.Metadata))
			};
		}

		return NoContent();
	}

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id, CancellationToken token = default)
	{
		var result = await _movieService.Delete(id, token);
		if (result.IsFailed)
			return NotFound(new { result.GetFirstError().Message });

		return NoContent();
	}

	[Authorize]
	[HttpPost("{id}/reviews")]
	public async Task<IActionResult> AddReview([FromRoute] Guid id, [FromBody] CreateUpdateReviewRequest request, CancellationToken token = default)
	{
		var result = await _movieService.AddReview(id, User.GetUserId(), request.ToDto(), token);
		if (result.IsFailed)
		{
			return result.GetFirstError() switch
			{
				NotFoundError notFound => NotFound(new { notFound.Message }),
				ValidationError valErr => ValidationProblem(ModelState.AddValidationErrors(valErr.Metadata))
			};
		}

		return Ok();
	}

	[Authorize(Roles = RoleNames.Admin)]
	[HttpPost("{id}/actors/{actorId}")]
	public async Task<IActionResult> AddActor(Guid id, Guid actorId, CancellationToken token = default)
	{
		var result = await _movieService.AddRemoveActor(id, actorId, DatabaseOperation.Add, token);
		if (result.IsFailed)
		{
			return result.GetFirstError() switch
			{
				NotFoundError notFound => NotFound(new { notFound.Message }),
				AlreadyAddedActorError err => BadRequest(new { err.Message })
			};
		}

		return NoContent();
	}

	[Authorize(Roles = RoleNames.Admin)]
	[HttpDelete("{id}/actors/{actorId}")]
    public async Task<IActionResult> RemoveActor(Guid id, Guid actorId, CancellationToken token = default)
    {
        var result = await _movieService.AddRemoveActor(id, actorId, DatabaseOperation.Remove, token);
        if (result.IsFailed)
        {
            return result.GetFirstError() switch
            {
                NotFoundError notFound => NotFound(new { notFound.Message }),
                AlreadyAddedActorError err => BadRequest(new { err.Message })
            };
        }

        return NoContent();
    }
}