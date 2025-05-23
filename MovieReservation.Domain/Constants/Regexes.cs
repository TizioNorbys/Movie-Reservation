namespace MovieReservation.Application.Constants;

public static class Regexes
{
	public const string MovieRating = @"\d?\.?\d*,\d?\.?\d?";
	public const string Word = @"^\w{2,}";
	public const string Range = @"\d*,\d*";
	public const string List = @"(\w+,?)+";
}