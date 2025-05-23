namespace MovieReservation.Application.DTOs.Reservation;

public record CreateReservationDto(Guid ShowtimeId, IEnumerable<Guid> SeatsIds);