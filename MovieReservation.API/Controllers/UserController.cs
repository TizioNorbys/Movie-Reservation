using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.Application.Interfaces.Services;

namespace MovieReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
	private readonly IUserService _userService;

    public UserController(IUserService userService)
	{
		_userService = userService;
	}

	[Authorize]
	[HttpGet("reservations")]
	public async Task<IActionResult> GetReservations(CancellationToken token = default)
	{
		var reservations = await _userService.GetReservations(User.GetUserId(), token);
		return reservations.Any() ? Ok(reservations) : NotFound(new { Message = "No result" });
	}
}