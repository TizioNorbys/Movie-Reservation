namespace MovieReservation.Application.Options;

public class DateOnlyOptions
{
    public string Format { get; set; } = null!;

    public DateOnly Min { get; set; }
}