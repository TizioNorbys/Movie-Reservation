using System.Reflection;

namespace MovieReservation.Application.Constants;

public static class Constant<TConstants>
    where TConstants : class
{
    private static readonly string?[] values;

    static Constant()
    {
        // get all const
        values = typeof(TConstants).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => f.GetRawConstantValue() as string)
            .ToArray();
    }

    public static string?[] GetValues() => values;

    public static bool IsDefined(string value) => values.Contains(value);
}