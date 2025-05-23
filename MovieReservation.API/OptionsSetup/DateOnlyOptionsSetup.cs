using Microsoft.Extensions.Options;
using MovieReservation.Application.Options;

namespace MovieReservation.API.OptionsSetup;

public class DateOnlyOptionsSetup : IConfigureOptions<DateOnlyOptions>
{
    private readonly IConfiguration _configuration;

    public DateOnlyOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DateOnlyOptions options)
    {
        string minValue = _configuration["DateOnly:Min"]!;
        string format = _configuration["DateOnly:Format"]!;

        options.Format = format;
        options.Min = DateOnly.ParseExact(minValue, format);
    }
}