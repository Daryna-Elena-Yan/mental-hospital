using System.Text.Json;
namespace Mental_Hospital.Services;

public class FileService
{
    public static void Serialize<T>(List<T> objects)
    {
        string jsonString = JsonSerializer.Serialize(objects);
        File.WriteAllText($"{typeof(T).Name}.json", jsonString);
    }
    public static List<T>? Deserialize<T>()
    {
        return JsonSerializer.Deserialize<List<T>>(GetString<T>() ?? string.Empty);
    }

    public static string? GetString<T>()
    {
        return File.ReadAllText($"{typeof(T).Name}.json");
    }
}

