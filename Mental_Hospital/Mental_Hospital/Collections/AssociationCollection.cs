using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Collections;



public class AssociationCollection<T> : ICollection<T> where T : IEntity
{
    private readonly IServiceProvider _serviceProvider;
    private List<Guid> _ids = [];
    
    [JsonIgnore]
    private List<T> _objects = [];

    public AssociationCollection(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AssociationCollection(List<Guid>? deserialize)
    {
        _ids = deserialize;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _objects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item) 
    {
        var storageAction = _serviceProvider.GetService<IAssociationAction<T>>();
        storageAction?.OnAdd(item); 
        
        _objects.Add(item);
        _ids.Add(item.Id);
    }

    public void Clear()
    {
        var storageAction = _serviceProvider.GetService<IAssociationAction<T>>();
        foreach (var obj in _objects)
        {       
            storageAction?.OnClear(obj); 
        }
        
        
        _objects.Clear();
        _ids.Clear();
    }

    public bool Contains(T item)
    {
        return _objects.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _objects.CopyTo(array, arrayIndex);
        //TODO copy indexes as well  _ids.CopyTo(, arrayIndex);
    }

    public bool Remove(T item)
    {
        var storageAction = _serviceProvider.GetService<IAssociationAction<T>>();
        storageAction?.OnDelete(item); 
        
        _objects.Remove(item);
        return _ids.Remove(item.Id);
    }
    
    public void RestoreObjects(Func<Guid, T> func)
    {
        foreach (var id in _ids)
        {
            _objects.Add(func(id));
        }
    }

    public List<Guid> GetIds => _ids;
    public int Count => _objects.Count;
    public bool IsReadOnly => false;
}


public class AssociationCollectionJsonConverter<T> : JsonConverter<AssociationCollection<T>> where T : IEntity
{
    public override AssociationCollection<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => 
        new(JsonSerializer.Deserialize<List<Guid>>(reader.GetString() ?? string.Empty, options));

    public override void Write(
        Utf8JsonWriter writer,
        AssociationCollection<T> collection,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(JsonSerializer.Serialize(collection.GetIds, options));
    }
}