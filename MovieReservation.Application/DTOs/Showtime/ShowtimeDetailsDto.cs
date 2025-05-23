using MovieReservation.Domain.Abstractions.DTOs;

namespace MovieReservation.Application.DTOs.Showtime;

public record ShowtimeDetailsDto(DateTime Timestamp, int HallNumber, int AvailableSeats, IEnumerable<ISeatStatusDto> SeatsStatus);