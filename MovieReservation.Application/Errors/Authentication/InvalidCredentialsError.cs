using FluentResults;

namespace MovieReservation.Application.Errors.Authentication;

public class InvalidCredentialsError : Error
{
    public InvalidCredentialsError()
        : base("Invalid email or password")
    {
    }
}