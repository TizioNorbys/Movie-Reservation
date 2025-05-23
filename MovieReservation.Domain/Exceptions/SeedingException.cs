namespace MovieReservation.Domain.Exceptions;

public class SeedingException : Exception
{ 
	public SeedingException()
		: base("Fail to seed admin user")
	{
	}
}