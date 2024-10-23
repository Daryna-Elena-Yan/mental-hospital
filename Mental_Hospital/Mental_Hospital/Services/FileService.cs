using System.Text.Json;
namespace Mental_Hospital.Services;

public class FileService
{
    static void Serialize<T>(List<T> objects)
    {
        string jsonString = JsonSerializer.Serialize(objects);
        File.WriteAllText($"{typeof(T).Name}.json", jsonString);
    }
    static List<T>? Deserialize<T>()
    {
        string json = File.ReadAllText($"{typeof(T).Name}.json");
        return JsonSerializer.Deserialize<List<T>>(json);
    }
}

