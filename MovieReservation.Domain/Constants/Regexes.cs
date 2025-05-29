namespace MovieReservation.Application.Constants;

public static class Regexes
{
	public const string MovieRating = @"\d?\.?\d*,\d?\.?\d?";
	public const string Word = @"^[a-zA-Z]{2,}";
	public const string List = @"([a-zA-Z]+,?)+";
	public const string Range = @"\d*,\d*";
}