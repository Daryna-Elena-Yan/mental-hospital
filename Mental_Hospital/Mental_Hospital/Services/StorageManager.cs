using System.Collections;
using System.Text.Json;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Services;

public class StorageManager
{
    private readonly Dictionary<Type, IStorage> _storages = new();
 

    public StorageManager(IEnumerable<IStorage> storages)
    {
        foreach (var stor in storages)
        {
            _storages.Add(stor.GetType().GenericTypeArguments[0], stor);
        }
    }

    public Storage<T> GetStorage<T>() where T : IEntity
    {
        return (_storages[typeof(T)] as Storage<T>)!;
    } 
    
    public class SerializeItem
    {
        public string? TypeName { get; set; }
        public string ContentJson { get; set; }
    }
    
    public void Serialize()
    {
        var dataList = _storages.Select(i => new SerializeItem()
        {
            TypeName = i.Key.FullName,
            ContentJson = i.Value.Serialize()
        }).ToList();

        //TODO mark storage type in resulting file
        var serializedString = JsonSerializer.Serialize(dataList);
        File.WriteAllText("data.json",serializedString);
    }
    public  void Deserialize()
    {
        var dataList = JsonSerializer.Deserialize<List<SerializeItem>>(GetJsonString() ?? string.Empty);
        foreach (var serializeItem in dataList)
        {
            _storages[Type.GetType(serializeItem.TypeName)].Deserialize(serializeItem.ContentJson);
        }

        foreach (var stor in _storages.Values)
        {
            stor.RestoreAllConnections();
        }
        
    }

    public static string? GetJsonString()
    {
        return File.ReadAllText("data.json");
    }
}

