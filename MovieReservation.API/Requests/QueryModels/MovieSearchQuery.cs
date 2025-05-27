using System.Globalization;
using System.Text;
using MovieReservation.Application.DTOs.MovieSearch;
using MovieReservation.Domain.Constants;

namespace MovieReservation.API.Requests.QueryModels;

public class MovieSearchQuery
{
    public string? Title { get; init; }
    public string? Rating { get; init; }
    public string? Genres { get; init; }
    public string? Runtime { get; init; }
    public string? Release_year { get; init; }
    public string? Language { get; init; }
    public string? Country { get; init; }

    public string Validate()
    {
        StringBuilder queryBuilder = new();

        queryBuilder = Title is null ? queryBuilder : queryBuilder.Append($"title={Uri.EscapeDataString(Title)}");
        queryBuilder = Rating is null ? queryBuilder : queryBuilder.Append(QueryValidator.ValidateMovieRating(Rating));
        queryBuilder = Genres is null ? queryBuilder : queryBuilder.Append(QueryValidator.ValidateListProperty<MovieSearchQuery, MovieGenres>(query => query.Genres!, Genres));
        queryBuilder = Runtime is null ? queryBuilder : queryBuilder.Append(QueryValidator.ValidateRangeProperty<MovieSearchQuery>(query => query.Runtime!, Runtime, min: 1, max: 100_000));
        queryBuilder = Release_year is null ? queryBuilder : queryBuilder.Append(QueryValidator.ValidateRangeProperty<MovieSearchQuery>(query => query.Release_year!, Release_year, min: 0, max: DateTime.UtcNow.Year));
        queryBuilder = Language is null ? queryBuilder : queryBuilder.Append(QueryValidator.ValidateWordProperty<MovieSearchQuery, Languages>(query => query.Language!, Language));
        queryBuilder = Country is null ? queryBuilder : queryBuilder.Append(QueryValidator.ValidateWordProperty<MovieSearchQuery, Countries>(query => query.Country!, Country));

        return queryBuilder.ToString().TrimStart('&');
    }

    public MovieSearchFiltersDto ToDto()
    {
        static double[] GetRangeValues(string input)
        {
            string[] values = input.Split(',');
            double min = double.Parse(values[0], CultureInfo.InvariantCulture);
            double max = double.Parse(values[1], CultureInfo.InvariantCulture);
            return [min, max];
        }

        var ratingRange = Rating is not null ? GetRangeValues(Rating) : null;
        var runtimeRange = Runtime is not null ? GetRangeValues(Runtime) : null;
        var yearRange = Release_year is not null ? GetRangeValues(Release_year) : null;

        return new MovieSearchFiltersDto
        {
            Title = Title,
            MinRating = ratingRange?[0],
            MaxRating = ratingRange?[1],
            Genres = Genres?.Split(","),
            MinRuntime = (int?)runtimeRange?[0],
            MaxRuntime = (int?)runtimeRange?[1],
            MinYear = (int?)yearRange?[0],
            MaxYear = (int?)yearRange?[1],
            Language = Language,
            Country = Country
        };
    }
}