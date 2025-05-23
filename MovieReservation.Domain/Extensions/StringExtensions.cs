using MovieReservation.Application.Constants;

namespace MovieReservation.Domain.Extensions;

public static class StringExtensions
{
    public static void ThrowIfNotDefined<TConstants>(this string value, string paramName)
        where TConstants : class
    {
        if (!Constant<TConstants>.IsDefined(value.ToLower().FirstLetterToUpper()))
            throw new ArgumentException($"{value} is not a valid {paramName}");
    }

    public static string FirstLetterToUpper(this string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return value;

		if (value.Length == 1)
			return value.ToUpper();

		return string.Concat(char.ToUpper(value[0]), value[1..]);
	}
}