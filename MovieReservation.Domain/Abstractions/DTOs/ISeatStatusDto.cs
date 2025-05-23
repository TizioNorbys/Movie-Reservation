namespace MovieReservation.Domain.Abstractions.DTOs;

public interface ISeatStatusDto
{
    int Number { get; init; }
    bool IsReserved { get; init; }
}