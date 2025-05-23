using FluentResults;

namespace MovieReservation.Application.Errors.Reservation;

public class TooManySeatsError : Error
{
    public TooManySeatsError()
        : base("It is possible to reserve a maximum of 10 seats")
    {
    }
}