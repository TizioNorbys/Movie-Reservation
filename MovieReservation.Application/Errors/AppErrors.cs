using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using MovieReservation.Application.Errors.Authentication;
using MovieReservation.Application.Errors.Authentication.Identity;
using MovieReservation.Application.Errors.Authorization;
using MovieReservation.Application.Errors.Movie;
using MovieReservation.Application.Errors.Reservation;
using MovieReservation.Application.Errors.Review;

namespace MovieReservation.Application.Errors;

public static class AppErrors
{
    public static InvalidCredentialsError InvalidCredentials { get; } = new();

    public static ForbidError Forbid { get; } = new();

    public static AlreadyAddedActorError AlreadyAddedActor { get; } = new();

    public static NoSeatsError NoSeats { get; } = new();

    public static WrongHallError WrongHall { get; } = new();

    public static TooManySeatsError TooManySeats { get; } = new();

    public static TooLateToCancelError TooLateToCancel { get; } = new();

    public static AlreadyReactedError AlreadyReacted => new();

    public static NotFoundError NotFound(Type type) => new(type);

    public static ValidationError Validation(IEnumerable<ValidationFailure> errors) => new(errors);

    public static UserManagerError UserManager(string message, IEnumerable<IdentityError> errors) => new(message, errors);

    public static UserRolesUpdateError UserRolesUpdate(IEnumerable<IdentityError> errors) => new(errors);

    public static InvalidRoleError InvalidRole(string roleName) => new(roleName);

    public static MovieSearchError MovieSearch(object value) => new(value);

    public static AlreadyInRoleError AlreadyInRole(string roleName) => new(roleName);

    public static NotInRoleError NotInRole(string roleName) => new(roleName);

    public static AlreadyReservedError AlreadyReserved(int seatNumber) => new(seatNumber);
}