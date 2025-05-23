namespace MovieReservation.Application.Utility;

public static class PathHelper
{
    private static readonly string BaseDirectoryPath = AppContext.BaseDirectory;

    public static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(BaseDirectoryPath, relativePath);
    }
}