using System.Collections;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Collections;

public interface IAssociationCollection
{
    void RestoreObjects(IServiceProvider serviceProvider);


}

public class AssociationCollection<T> : IAssociationCollection, ICollection<T> where T : IEntity
{
    [JsonIgnore]
    private IServiceProvider _serviceProvider;
    private List<Guid> _ids = [];
    
    [JsonIgnore]
    private List<T> _objects = [];  

    public Action<T>? OnDelete { get; set; }
    public Action<T>? OnAdd { get; set; }
    
    
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
        OnAdd?.Invoke(item);  
        
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
    
    public void RestoreObjects(IServiceProvider serviceProvider)
    {
        _objects.Clear();
        _serviceProvider = serviceProvider;
        
        var type = typeof(T);
        var storage = _serviceProvider.FindStorage(type);
      
        foreach (var id in _ids)
        {
            _objects.Add((T)storage.GetById(id));
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