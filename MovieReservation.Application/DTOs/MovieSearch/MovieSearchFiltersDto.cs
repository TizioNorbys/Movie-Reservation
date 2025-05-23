namespace MovieReservation.Application.DTOs.MovieSearch;

public record MovieSearchFiltersDto
{
    public required string? Title { get; init; }
    public required double? MinRating { get; init; }
    public required double? MaxRating { get; init; }
    public required string[]? Genres { get; init; }
    public required int? MinRuntime { get; init; }
    public required int? MaxRuntime { get; init; }
    public required int? MinYear { get; init; }
    public required int? MaxYear { get; init; }
    public required string? Language { get; init; }
    public required string? Country { get; init; }
}