using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mental_Hospital.Models;
using Microsoft.Extensions.DependencyInjection;
namespace Mental_Hospital.Collections;

public class AssociationDictionary<T> : IAssociationCollection,IDictionary<Guid,T> where T : IEntity
{
    [JsonIgnore]
    private IEntity _parent;

    [JsonIgnore] 
    private IServiceProvider _serviceProvider;
    public AssociationDictionary(IEntity parent, IServiceProvider serviceProvider)
    {
        _parent = parent;
        _serviceProvider = serviceProvider;
    }
    public AssociationDictionary(List<Guid>? deserialize)
    {
        Keys = deserialize;
    }
    public void RestoreObjects(IEntity parent, IServiceProvider serviceProvider)
    {
        Values.Clear();
        _serviceProvider = serviceProvider;
        _parent = parent;

        var type = typeof(T);
        var storage = _serviceProvider.FindStorage(type);

        foreach (var id in Keys)
        {
            Values.Add((T)storage.GetById(id));
        }    
    }

    public IEnumerator<KeyValuePair<Guid, T>> GetEnumerator()
    {
        return Keys.Zip(Values, (key, value) => new KeyValuePair<Guid, T>(key, value)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<Guid, T> item)
    {
        if (Keys.Contains(item.Key))
            return;    
        Values.Add(item.Value);
        Keys.Add(item.Key);
        //Береженого бог бережет @ Ян
        var dictionaryType = typeof(AssociationDictionary<>).MakeGenericType(_parent.GetType());
        var dictionaryProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == dictionaryType);
        if (dictionaryProp != null)
        {
            var dictionary = dictionaryProp.GetMethod.Invoke(item.Value, null);

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
            var collection = collectionProp.GetMethod.Invoke(item.Value, null);

            var addMethod = collectionType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            addMethod.Invoke(collection, new[] { _parent });
        }
        var propReferences = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name==_parent.GetType().Name);
            
        foreach (var propertyInfo in propReferences)
        {
            var instance = Convert.ChangeType(propertyInfo.GetMethod.Invoke(item.Value, null),_parent.GetType());
            propertyInfo.SetMethod.Invoke(item.Value,  new [] {instance});
        }
    }

    public void Clear()
    {
       
        foreach (var obj in Values)
        {
            
        }    
    }

    public bool Contains(KeyValuePair<Guid, T> item)
    {
        return Keys.Contains(item.Key) && Values.Contains(item.Value);
    }

    public void CopyTo(KeyValuePair<Guid, T>[] array, int arrayIndex)
    {
        Keys.Zip(Values, (key, value) => new KeyValuePair<Guid, T>(key, value)).ToList().CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<Guid, T> item)
    {
        if (!Contains(item))
            return false;
        var dictionaryType = typeof(AssociationDictionary<>).MakeGenericType(_parent.GetType());
        var dictionaryProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == dictionaryType);
        if (dictionaryProp != null)
        {
            var dictionary = dictionaryProp.GetMethod.Invoke(item.Value, null);

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
                storage.DeleteById(item.Key);
            }
            var collection = collectionProp.GetMethod.Invoke(item.Value, null);

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
                storage.DeleteById(item.Key);
            }
            propertyInfo.SetMethod.Invoke(item.Value,new []{(object)null});
        }
        Keys.Remove(item.Key);
        Values.Remove(item.Value);
        return true;
    }

    public int Count => Values.Count;
    public bool IsReadOnly => false;
    public void Add(Guid key, T value)
    {
        if (Keys.Contains(key))
            return;    
        Values.Add(value);
        Keys.Add(key);
        var dictionaryType = typeof(AssociationDictionary<>).MakeGenericType(_parent.GetType());
        var dictionaryProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == dictionaryType);
        if (dictionaryProp != null)
        {
            var dictionary = dictionaryProp.GetMethod.Invoke(value, null);

            var addMethod = dictionaryType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeof(Guid), _parent.GetType());
            var keyValuePair = Activator.CreateInstance(keyValuePairType, _parent.Id, _parent);            
            addMethod.Invoke(dictionary, new[] { keyValuePair });
        }
        var collectionType = typeof(AssociationCollection<>).MakeGenericType(_parent.GetType());

        var collectionProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == collectionType);
        if (collectionProp != null)
        {
            var collection = collectionProp.GetMethod.Invoke(value, null);

            var addMethod = collectionType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            addMethod.Invoke(collection, new[] { _parent });
        }
        var propReferences = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name==_parent.GetType().Name);
            
        foreach (var propertyInfo in propReferences)
        {
            var instance = Convert.ChangeType(propertyInfo.GetMethod.Invoke(value, null),_parent.GetType());
            propertyInfo.SetMethod.Invoke(value,  new [] {instance});
        }    }

    public bool ContainsKey(Guid key)
    {
        return Keys.Contains(key);
    }

    public bool Remove(Guid key)
    {
        if (!ContainsKey(key))
            return false;
        var item = Values.First(v => v.Id.Equals(key));
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
        var collectionType = typeof(AssociationCollection<>).MakeGenericType(_parent.GetType());

        var collectionProp = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(c => c.PropertyType == collectionType);
        if (collectionProp != null)
        {
            var collection = collectionProp.GetMethod.Invoke(item, null);

            var removeMethod = collectionType.GetMethod("Remove", BindingFlags.Instance | BindingFlags.Public);
            removeMethod.Invoke(collection, new[] { _parent });
        }
        var propReferences = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name==_parent.GetType().Name);
            
        foreach (var propertyInfo in propReferences)
        {
            propertyInfo.SetMethod.Invoke(item, new [] { (object)null });
        }
        Keys.Remove(key);
        Values.Remove(item);
        return true;    }

    public bool TryGetValue(Guid key, [MaybeNullWhen(false)] out T value)
    {
        if (Keys.Contains(key))
        {
            value=Values.First(v=>v.Id.Equals(key));
            return true;
        }
        value = default;
        return false;
    }
    public T this[Guid key]
    {
        get => Values.First(v=>v.Id.Equals(key));
        set
        {
            Values.Remove(Values.First(v=>v.Id.Equals(key)));
            Keys.Remove(key);
            Values.Add(value);
            Keys.Add(value.Id);
        }
    }
    public ICollection<Guid> Keys { get; }=new List<Guid>();
   [JsonIgnore]
   public ICollection<T> Values { get; }=new List<T>();
}
public class AssociationDictionaryJsonConverter<T> : JsonConverter<AssociationDictionary<T>> where T : IEntity
{
    public override AssociationDictionary<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        new(JsonSerializer.Deserialize<List<Guid>>(reader.GetString() ?? string.Empty, options));

    public override void Write(
        Utf8JsonWriter writer,
        AssociationDictionary<T> dictionary,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(JsonSerializer.Serialize(dictionary.Keys, options));
    }
}