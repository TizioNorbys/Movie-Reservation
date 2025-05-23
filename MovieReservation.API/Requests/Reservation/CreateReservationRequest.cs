namespace MovieReservation.API.Requests.Reservation;

public record CreateReservationRequest(Guid ShowtimeId, IEnumerable<Guid> SeatsIds);