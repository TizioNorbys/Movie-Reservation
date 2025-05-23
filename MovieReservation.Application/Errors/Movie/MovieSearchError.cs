using FluentResults;

namespace MovieReservation.Application.Errors.Movie;

public class MovieSearchError : Error
{
	public MovieSearchError(object value)
		: base($"No movies found for {value}")
	{
	}
}