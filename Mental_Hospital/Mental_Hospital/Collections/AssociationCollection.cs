using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Collections;

public interface IAssociationCollection
{
    void RestoreObjects(IEntity parent, IServiceProvider serviceProvider);
}

public class AssociationCollection<T> : IAssociationCollection, ICollection<T> where T : IEntity
{
    [JsonIgnore]
    private IEntity _parent;

    [JsonIgnore] 
    private IServiceProvider _serviceProvider;
    private List<Guid> _ids = [];

    [JsonIgnore] private List<T> _objects = [];

    public Action<T>? OnDelete { get; set; }
    public Action<T>? OnAdd { get; set; }


    public AssociationCollection(IEntity parent, IServiceProvider serviceProvider)
    {
        _parent = parent;
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
        if (_ids.Contains(item.Id))
            return;
        
        _objects.Add(item);
        _ids.Add(item.Id);

        var collectionType = typeof(AssociationCollection<>).MakeGenericType(_parent.GetType());

        var collectionProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == collectionType);
        if (collectionProp != null)
        {
            var collection = collectionProp.GetMethod.Invoke(item, null);

            var addMethod = collectionType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            addMethod.Invoke(collection, new[] { _parent });
        }

        //   Nurse.Rooms.Add(room);
//Room room = item as Room;

        //room.Nurses.Add(_parent);


        //TODO check if object is already in the list (can happen on reverse connection)

    }


    public void Clear()
    {
        var storageAction = _serviceProvider.GetService<IAssociationAction<T>>();
        foreach (var obj in _objects)
        {
            storageAction?.OnDelete(obj);
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
    }

    public bool Remove(T item)
    {
        var storageAction = _serviceProvider.GetService<IAssociationAction<T>>();
        storageAction?.OnDelete(item);

        _objects.Remove(item);
        return _ids.Remove(item.Id);
    }

    public void RestoreObjects(IEntity parent, IServiceProvider serviceProvider)
    {
        _objects.Clear();
        _serviceProvider = serviceProvider;
        _parent = parent;

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