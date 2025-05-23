namespace MovieReservation.API.Requests.Movie;

public record CreateUpdateMovieRequest(
    string Title,
    string Description,
    string Plot,
    int ReleaseYear,
    string OriginalLanguage,
    string Country,
    int RunningTime,
    double Rating,
    IEnumerable<string> Genres);