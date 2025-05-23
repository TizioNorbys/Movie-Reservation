namespace MovieReservation.Application.DTOs.Actor;

public record CreateUpdateActorDto(string FullName, DateOnly BirthDate);