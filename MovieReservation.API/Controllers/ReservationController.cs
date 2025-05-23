using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.API.Requests.Reservation;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Errors.Authorization;
using MovieReservation.Application.Errors.Reservation;
using MovieReservation.Application.Interfaces.Services;

namespace MovieReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
	private readonly IReservationService _reservationService;

	public ReservationController(IReservationService reservationService)
	{
		_reservationService = reservationService;
	}

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Create(CreateReservationRequest request, CancellationToken token = default)
	{
		var user = User;
		var result = await _reservationService.Create(User.GetUserId(), request.ToDto(), token);
		if (result.IsFailed)
		{
            return result.GetFirstError() switch
			{
				NotFoundError notFound => NotFound(new { notFound.Message }),
				NoSeatsError noSeats => BadRequest(new { noSeats.Message}),
                WrongHallError wrongHall => BadRequest(new { wrongHall.Message }),
				AlreadyReservedError err => Conflict(new { err.Message })
            };
		}

        return Created();
	}

	[Authorize]
	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id, CancellationToken token = default)
	{
		var result = await _reservationService.Delete(id, User.GetUserId(), token);
        if (result.IsFailed)
        {
			return result.GetFirstError() switch
			{
				NotFoundError notFound => NotFound(new { notFound.Message }),
				ForbidError => Forbid(),
				TooLateToCancelError tooLateErr => Conflict(new { tooLateErr.Message })
			};
        }

		return NoContent();
    }
}