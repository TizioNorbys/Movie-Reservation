namespace MovieReservation.Domain.Exceptions;

public class SeedingException : Exception
{ 
	public SeedingException(string message = "Seeding failed")
		: base()
	{
	}
}