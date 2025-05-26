using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using MovieReservation.Application.Constants;
using MovieReservation.Application.DTOs.MovieSearch;
using MovieReservation.Application.Extensions;
using MovieReservation.Domain.Constants;
using MovieReservation.Domain.Extensions;

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

        queryBuilder = Rating is null ? queryBuilder : queryBuilder.Append(ValidateRating());

        queryBuilder = Genres is null ? queryBuilder : queryBuilder.Append(ValidateListProperty<MovieGenres>(query => query.Genres!, Genres));

        queryBuilder = Runtime is null ? queryBuilder : queryBuilder.Append(ValidateRangeProperty(query => query.Runtime!, Runtime, min: 1, max: 100_000));

        queryBuilder = Release_year is null ? queryBuilder : queryBuilder.Append(ValidateRangeProperty(query => query.Release_year!, Release_year, min: 0, max: DateTime.UtcNow.Year));

        queryBuilder = Language is null ? queryBuilder : queryBuilder.Append(ValidateWordProperty<Languages>(query => query.Language!, Language));

        queryBuilder = Country is null ? queryBuilder : queryBuilder.Append(ValidateWordProperty<Countries>(query => query.Country!, Country));

        return queryBuilder.ToString().TrimStart('&');
    }

    private string ValidateRating()
    {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;

        var match = Regex.Match(Rating!, Regexes.MovieRating);
        if (!match.Success)
            return "&rating=1.0,10.0";

        var values = match.Value.Split(',');
        string left = values[0];
        string right = values[1];
        double from, to;

        if (left != string.Empty && left != ".")
        {
            if (double.TryParse(left, invariantCulture, out from))
            {
                if (from < 1 || from > 10)
                    from = 1.0;
                else
                    from = Convert.ToDouble(from.ToString("N1"));
            }
        }
        else from = 1.0;

        if (right != string.Empty && right != ".")
        {
            if (double.TryParse(right, invariantCulture, out to))
            {
                if (to < 1 || to > 10)
                    to = 10.0;
            }
        }
        else to = 10.0;

        if (from > to)
            (from, to) = (to, from);

        return $"&rating={from.ToString(invariantCulture)},{to.ToString(invariantCulture)}";
    }

    private static string ValidateWordProperty<TConstants>(Expression<Func<MovieSearchQuery, string>> propertySelector, string input)
        where TConstants : class
    {
        string propertyName = propertySelector.GetPropertyName().ToLower();

        var match = Regex.Match(input, Regexes.Word);
        if (!match.Success)
            return string.Empty;

        var matchValue = match.Value.ToLower();
        if (!Constant<TConstants>.IsDefined(matchValue.FirstLetterToUpper()))
            return string.Empty;

        return $"&{propertyName}={matchValue}";
    }

    private static string ValidateRangeProperty(Expression<Func<MovieSearchQuery, string>> propertySelector, string input, int min, int max)
    {
        string propertyName = propertySelector.GetPropertyName().ToLower();

        var match = Regex.Match(input, Regexes.Range);
        if (!match.Success)
            return $"&{propertyName}={min},{max}";

        var values = match.Value.Split(',');
        string left = values[0];
        var right = values[1];
        int from, to;

        if (left != string.Empty)
        {
            if (int.TryParse(left, out from))
            {
                if (from < min || from > max)
                    from = min;
            }
        }
        else from = min;

        if (right != string.Empty)
        {
            if (int.TryParse(right, out to))
            {
                if (to < min || to > max)
                    to = max;
            }
        }
        else to = max;

        if (from > to)
            (from, to) = (to, from);

        return $"&{propertyName}={from},{to}";
    }

    private static string ValidateListProperty<TContants>(Expression<Func<MovieSearchQuery, string>> propertySelector, string input)
        where TContants : class
    {
        string propertyName = propertySelector.GetPropertyName().ToLower();

        var match = Regex.Match(input, Regexes.List);
        if (!match.Success)
            return string.Empty;

        var values = match.Value.TrimEnd(',').Split(',').Select(s => s.ToLower().FirstLetterToUpper());
        StringBuilder queryParam = new();

        foreach (string value in values)
        {
            if (!Constant<TContants>.IsDefined(value))
                continue;

            if (queryParam.Length == 0)
            {
                queryParam.Append($"&{propertyName}={value.ToLower()},");
                continue;
            }

            queryParam.Append($"{value.ToLower()},");
        }

        return queryParam.ToString().TrimEnd(',');
    }

    public MovieSearchFiltersDto ToDto()
    {
        static double[]? GetRangeValues(string? input)
        {
            if (input is null)
                return null;

            string[] values = input.Split(',');
            double min = double.Parse(values[0], CultureInfo.InvariantCulture);
            double max = double.Parse(values[1], CultureInfo.InvariantCulture);
            return [min, max];
        }

        var ratingRange = GetRangeValues(Rating);
        var runtimeRange = GetRangeValues(Runtime);
        var yearRange = GetRangeValues(Release_year);

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