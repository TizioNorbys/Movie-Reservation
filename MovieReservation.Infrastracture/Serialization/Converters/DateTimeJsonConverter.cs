using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieReservation.Infrastracture.Serialization.Converters;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    private static readonly string format = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, format, null);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(format));
    }
}