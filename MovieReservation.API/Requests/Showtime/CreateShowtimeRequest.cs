namespace MovieReservation.API.Requests.Showtime;

public record CreateShowtimeRequest(DateTime Timestamp, Guid HallId, Guid MovieId);