using FluentResults;
using MovieReservation.Application.DTOs.Auth;

namespace MovieReservation.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result> SignUp(RegistrationDto request, CancellationToken token = default);

    Task<Result<string>> SignIn(LoginDto request);

    Task<Result> AddToRole(Guid id, string role);

    Task<Result> RemoveFromRole(Guid id, string role);

    Task<Result> Delete(Guid id);
}