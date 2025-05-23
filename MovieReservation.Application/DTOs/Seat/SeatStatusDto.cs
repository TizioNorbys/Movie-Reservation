using MovieReservation.Domain.Abstractions.DTOs;

namespace MovieReservation.Application.DTOs.Seat;

public record SeatStatusDto : ISeatStatusDto
{
    public required int Number { get; init; }
    public required bool IsReserved { get; init; }
}