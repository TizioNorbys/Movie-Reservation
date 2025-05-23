using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.API.Requests.Actor;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Constants;

namespace MovieReservation.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("api/[controller]")]
public class ActorController : ControllerBase
{
	private readonly IActorService _actorService;

	public ActorController(IActorService actorService)
	{
		_actorService = actorService;
	}

    [HttpPost]
	public async Task<IActionResult> Create(CreateUpdateActorRequest request, CancellationToken token = default)
	{
		var result = await _actorService.Create(request.ToDto(), token);
        if (result.IsFailed)
        {
            var error = result.GetFirstError();
            return ValidationProblem(ModelState.AddValidationErrors(error.Metadata));
        }

		return Created();
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken token = default)
    {
        var result = await _actorService.GetById(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpGet("{id}/movies")]
    public async Task<IActionResult> GetMovies(Guid id, CancellationToken token = default)
    {
        var result = await _actorService.GetMovies(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CreateUpdateActorRequest request, CancellationToken token = default)
    {
        var result = await _actorService.Update(id, request.ToDto(), token);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token = default)
    {
        var result = await _actorService.Delete(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return NoContent();
    }
}