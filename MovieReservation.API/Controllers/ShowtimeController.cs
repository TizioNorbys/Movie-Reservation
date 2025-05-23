using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.API.Requests.Showtime;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Constants;

namespace MovieReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimeController : ControllerBase
{
    private readonly IShowtimeService _showtimeService;

    public ShowtimeController(IShowtimeService showtimeService)
    {
        _showtimeService = showtimeService;
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateShowtimeRequest request, CancellationToken token = default)
    {
        var result = await _showtimeService.Create(request.ToDto(), token);
        if (result.IsFailed)
        {
            return result.GetFirstError() switch
            {
                NotFoundError notFound => NotFound(new { notFound.Message }),
                ValidationError valErr => ValidationProblem(ModelState.AddValidationErrors(valErr.Metadata))
            };
        }

        return Created();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken token = default)
    {
        var result = await _showtimeService.Get(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return Ok(result.Value);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateShowtimeRequest request, CancellationToken token = default)
    {
        var result = await _showtimeService.Update(id, request.ToDto(), token);
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken token = default)
    {
        var result = await _showtimeService.Delete(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return NoContent();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet("{id}/reservations")]
    public async Task<IActionResult> GetReservations(Guid id, CancellationToken token = default)
    {
        var result = await _showtimeService.GetReservations(id, token);
        if (result.IsFailed)
            return NotFound(new { result.GetFirstError().Message });

        return result.Value.Any() ? Ok(result.Value) : NotFound(new { Message = "No reservations" });
    }
}