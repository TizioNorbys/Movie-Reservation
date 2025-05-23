namespace MovieReservation.API.Requests.Actor;

public record CreateUpdateActorRequest(string FullName, DateOnly BirthDate);