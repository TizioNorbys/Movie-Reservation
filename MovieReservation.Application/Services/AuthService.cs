using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MovieReservation.Application.Constants;
using MovieReservation.Application.DTOs.Auth;
using MovieReservation.Application.Errors;
using MovieReservation.Application.Interfaces.Authentication;
using MovieReservation.Application.Interfaces.Services;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Entities;
using MovieReservation.Domain.Extensions;

namespace MovieReservation.Application.Services;

public class AuthService : IAuthService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IValidator<RegistrationDto> _registrationValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IJwtProvider jwtProvider,
        IValidator<RegistrationDto> registrationValidator,
        IValidator<LoginDto> loginValidator,
        UserManager<User> userManager,
        ILogger<AuthService> logger)
    {
        _jwtProvider = jwtProvider;
        _registrationValidator = registrationValidator;
        _loginValidator = loginValidator;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> SignUp(RegistrationDto request, CancellationToken token = default)
    {
        var valResult = await _registrationValidator.ValidateAsync(request, token);
        if (!valResult.IsValid)
        {
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.Validation(valResult.Errors));
        }

        User user = new(request.Email, request.FirstName, request.LastName);

        _logger.LogInformation("Adding new user to the database");
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            _logger.LogWarning("Registration failed. Unable to add the user to the database");
            return Result.Fail(AppErrors.UserManager("Error while creating the user", createResult.Errors));
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, RoleNames.User);
        if (!addRoleResult.Succeeded)
        {
            _logger.LogWarning("Unable to add {Role} role to user with {Id} id", RoleNames.User, user.Id);
            return Result.Fail(AppErrors.UserManager("Error adding user to role", addRoleResult.Errors));
        }

        return Result.Ok();
    }

    public async Task<Result<string>> SignIn(LoginDto request)
    {
        var valResult = _loginValidator.Validate(request);
        if (!valResult.IsValid)
        {
            _logger.LogWarning("Model validation failed. Errors: {@Errors}", valResult.Errors);
            return Result.Fail(AppErrors.InvalidCredentials);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogWarning("Login failed. Invalid credentials");
            return Result.Fail(AppErrors.InvalidCredentials);
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogWarning("Login failed. Invalid credentials");
            return Result.Fail(AppErrors.InvalidCredentials);
        }

        var token = await _jwtProvider.GenerateAsync(user);
        return token;
    }

    public async Task<Result> AddToRole(Guid id, string role)
    {
        if (!IsValidRole(role))
            return Result.Fail(AppErrors.InvalidRole(role));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            _logger.LogWarning("User with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(User)));
        }
        
        if (await _userManager.IsInRoleAsync(user, role))
        {
            _logger.LogWarning("Unable to add user to role. User with {Id} id is already in {Role} role", id, role);
            return Result.Fail(AppErrors.AlreadyInRole(role));
        }

        _logger.LogInformation("Adding user with {Id} id to {Role} role", id, role);
        var identityResult = await _userManager.AddToRoleAsync(user, role.ToLower().FirstLetterToUpper());
        if (!identityResult.Succeeded)
        {
            _logger.LogWarning("Failed to add user with {Id} id to {Role} role", id, role);
            return Result.Fail(AppErrors.UserRolesUpdate(identityResult.Errors));
        }

        return Result.Ok();
    }

    public async Task<Result> RemoveFromRole(Guid id, string role)
    {
        if (!IsValidRole(role))
            return Result.Fail(AppErrors.InvalidRole(role));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            _logger.LogWarning("User with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(User)));
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            _logger.LogWarning("Unable to remove user from role. User with {Id} id is not in {Role} role", id, role);
            return Result.Fail(AppErrors.NotInRole(role));
        }

        var identityResult = await _userManager.RemoveFromRoleAsync(user, role.ToLower().FirstLetterToUpper());
        if (!identityResult.Succeeded)
        {
            _logger.LogWarning("Failed to remove user with {id} id from {Role} role", id, role);
            return Result.Fail(AppErrors.UserRolesUpdate(identityResult.Errors));
        }

        return Result.Ok();
    }

    public async Task<Result> Delete(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            _logger.LogWarning("User with {Id} id not found", id);
            return Result.Fail(AppErrors.NotFound(typeof(User)));
        }

        _logger.LogInformation("Deleting user with {Id} id from the database", id);
        var identityResult = await _userManager.DeleteAsync(user);
        if (!identityResult.Succeeded)
        {
            _logger.LogWarning("Unable to delete user with {Id} id from the database", id);
            return Result.Fail(AppErrors.UserManager("Error while deleting the user", identityResult.Errors));
        }

        return Result.Ok();
    }
    
    private static bool IsValidRole(string roleName)
    {
        return Constant<RoleNames>.IsDefined(roleName) && roleName != RoleNames.User;
    }
}