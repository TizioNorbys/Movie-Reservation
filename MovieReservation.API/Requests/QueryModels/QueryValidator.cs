using MovieReservation.Application.Constants;
using MovieReservation.Application.Extensions;
using MovieReservation.Domain.Extensions;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace MovieReservation.API.Requests.QueryModels;

public static class QueryValidator
{
    public static string ValidateMovieRating(string rating)
    {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;

        var match = Regex.Match(rating, Regexes.MovieRating);
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

    public static string ValidateWordProperty<TQueryModel, TConstants>(Expression<Func<TQueryModel, string>> propertySelector, string input)
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

    public static string ValidateListProperty<TQueryModel, TContants>(Expression<Func<TQueryModel, string>> propertySelector, string input)
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

    public static string ValidateRangeProperty<TQueryModel>(Expression<Func<TQueryModel, string>> propertySelector, string input, int min, int max)
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
}