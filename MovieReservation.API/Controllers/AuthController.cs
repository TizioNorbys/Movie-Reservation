using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReservation.API.Extensions;
using MovieReservation.API.Requests.Auth;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Errors.Authentication.Identity;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> SignUp(RegistrationRequest request, CancellationToken token = default)
	{
		var result = await _authService.SignUp(request.ToDto(), token);
		if (result.IsFailed)
		{
			return result.GetFirstError() switch
			{
				ValidationError valErr => ValidationProblem(ModelState.AddValidationErrors(valErr.Metadata)),
				UserManagerError err => BadRequest(new { err.Message, err.Metadata })
			};
		}

		return Created();
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> SignIn(LoginRequest request)
	{
		var result = await _authService.SignIn(request.ToDto());
		if (result.IsFailed)
			return Unauthorized(new { result.GetFirstError().Message });

		return Ok(result.Value);
	}

	[Authorize(Roles = RoleNames.Admin)]
	[HttpPost("user/{id}/roles")]
	public async Task<IActionResult> AddToRole(Guid id, [FromQuery] string role)
	{
		var result = await _authService.AddToRole(id, role.ToLower().FirstLetterToUpper());
		if (result.IsFailed)
		{
			return result.GetFirstError() switch
			{
				InvalidRoleError invalidRole => BadRequest(new { invalidRole.Message }),
				NotFoundError notFound => NotFound(new { notFound.Message }),
				AlreadyInRoleError alreadyInRole => Conflict(new { alreadyInRole.Message }),
				UserRolesUpdateError err => StatusCode(500, new { err.Message, err.Metadata })
			};
		}

		return NoContent();
	}

	[Authorize(Roles = RoleNames.Admin)]
	[HttpDelete("user/{id}/roles")]
	public async Task<IActionResult> RemoveFromRole(Guid id, [FromQuery] string role)
	{
		var result = await _authService.RemoveFromRole(id, role.ToLower().FirstLetterToUpper());
		if (result.IsFailed)
		{
			return result.GetFirstError() switch
			{
				InvalidRoleError invalidRole => BadRequest(new { invalidRole.Message }),
				NotFoundError notFound => NotFound(new { notFound.Message }),
                NotInRoleError notInRole => NotFound(new { notInRole.Message }),
				UserRolesUpdateError err => StatusCode(500, new { err.Message, err.Metadata })
			};
		}

		return NoContent();
	}
}