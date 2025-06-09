using System.Text.Json;
using MovieReservation.Infrastracture.Serialization.Converters;

namespace MovieReservation.Infrastracture.Serialization;

public static class JsonReader
{
    private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    static JsonReader()
    {
        options.Converters.Add(new DateTimeJsonConverter());
    }

    public static List<T> ReadAndDeserialize<T>(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
    }
}