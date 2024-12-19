using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Collections;



public class AssociationCollection<T> : IAssociationCollection, ICollection<T> where T : IEntity
{
    [JsonIgnore]
    private IEntity _parent;

    [JsonIgnore] 
    private IServiceProvider _serviceProvider;
    private List<Guid> _ids = [];

    [JsonIgnore] private List<T> _objects = [];


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
        //Береженого бог бережет @ Ян
        var dictionaryType = typeof(AssociationDictionary<>).MakeGenericType(_parent.GetType());
        var dictionaryProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == dictionaryType);
        if (dictionaryProp != null)
        {
            var dictionary = dictionaryProp.GetMethod.Invoke(item, null);

            var addMethod = dictionaryType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(Guid), _parent.GetType());
            var keyValuePair = Activator.CreateInstance(keyValuePairType, _parent.Id, _parent);            
            addMethod.Invoke(dictionary, new[] { keyValuePair });
        }
        //конец цитаты
        var collectionType = typeof(AssociationCollection<>).MakeGenericType(_parent.GetType());

        var collectionProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == collectionType);
        if (collectionProp != null)
        {
            var collection = collectionProp.GetMethod.Invoke(item, null);

            var addMethod = collectionType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            addMethod.Invoke(collection, new[] { _parent });
        }
        
        var propReferences = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name==_parent.GetType().Name);
            
        foreach (var propertyInfo in propReferences)
        {
            var instance = Convert.ChangeType(propertyInfo.GetMethod.Invoke(item, null),_parent.GetType());
            propertyInfo.SetMethod.Invoke(item,  new [] {instance});
        }

        //   Nurse.Rooms.Add(room);
        //Room room = item as Room;
        //room.Nurses.Add(_parent);


        //TODO check if object is already in the list (can happen on reverse connection)

    }


    public void Clear()
    {
        
        foreach (var obj in _objects)
        {
           
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
        if (!_ids.Contains(item.Id))
            return false;
        _ids.Remove(item.Id);
        var dictionaryType = typeof(AssociationDictionary<>).MakeGenericType(_parent.GetType());
        var dictionaryProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == dictionaryType);
        if (dictionaryProp != null)
        {
            var dictionary = dictionaryProp.GetMethod.Invoke(item, null);

            var removeMethod = dictionaryType.GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);
            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(Guid), _parent.GetType());
            var keyValuePair = Activator.CreateInstance(keyValuePairType, _parent.Id, _parent);            
            removeMethod.Invoke(dictionary, new[] { keyValuePair });
        }  
        
        var compositionAttr = _parent.GetType().GetProperties().FirstOrDefault(x => x.GetCustomAttribute<CompositionAttribute>() != null);
        
        var collectionType = typeof(AssociationCollection<>).MakeGenericType(_parent.GetType());
        var collectionProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == collectionType);
        if (collectionProp != null)
        {
            
            if (compositionAttr != null)
            {
               var storage = _serviceProvider.FindStorage(typeof(T));
               storage.DeleteById(item.Id);
            }
            var collection = collectionProp.GetMethod.Invoke(item, null);

            var removeMethod = collectionType.GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);
            removeMethod.Invoke(collection, new[] { _parent });
        }
        var propReferences = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name==_parent.GetType().Name);
            
        foreach (var propertyInfo in propReferences)
        {
            if (compositionAttr != null)
            {
                var storage = _serviceProvider.FindStorage(typeof(T));
                storage.DeleteById(item.Id);
            }
            else
            {
                propertyInfo.SetMethod.Invoke(item,new []{(object)null});
            }
            
        }

        
        return _objects.Remove(item);
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