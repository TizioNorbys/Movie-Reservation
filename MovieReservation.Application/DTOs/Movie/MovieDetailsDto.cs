namespace MovieReservation.Application.DTOs.Movie;

public record MovieDetailsDto(
    string Title,
    string Description,
    string Plot,
    int ReleaseYear,
    string OriginalLanguage,
    string Country,
    int RunningTime,
    double Rating,
    IEnumerable<string> Genres)
{
}