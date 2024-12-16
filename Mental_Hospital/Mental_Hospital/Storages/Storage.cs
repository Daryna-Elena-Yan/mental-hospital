using System.Reflection;
using System.Text.Json;
using FluentValidation;
using Mental_Hospital.Collections;
using Mental_Hospital.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Storages;

public class Storage<T> : IStorage where T : IEntity
{
    private readonly IServiceProvider _serviceProvider;
    private List<T> _list = [];

    public Storage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void RegisterNew<TS>(TS t) where TS:T
    {
        var validator = _serviceProvider.GetService<IValidator<TS>>();
        if (validator is not null)
            validator?.ValidateAndThrow(t);
        else
            _serviceProvider.GetService<IValidator<T>>()?.ValidateAndThrow(t);
        
        var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        if (storageAction is not null)
            storageAction?.OnAdd(t);
        else
            _serviceProvider.GetService<IStorageAction<T>>()?.OnAdd(t);
        
        // if sp wont find service registration, nothong will be called
        _list.Add(t);
    }

    public void Delete<TS>(TS t) where TS:T
    {
        var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        if (storageAction is not null)
            storageAction?.OnDelete(t); 
        else
            _serviceProvider.GetService<IStorageAction<T>>()?.OnDelete(t);
        
        _list.Remove(t);
    }
    
    public void Update<TS>(TS tOld, TS tNew) where TS:T
    {
        RegisterNew(tNew);
        Delete(tOld);
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(_list);
    }

    public void Deserialize(string json)
    {
        _list = JsonSerializer.Deserialize<List<T>>(json)?? [];
    }

    public void RestoreAllConnections()
    {
        foreach ( dynamic obj in _list)
        {
            RestoreObjectConnections(obj);
        }
    }

    public object? GetById(Guid id)
    {
        return _list.FirstOrDefault(x => x.Id == id);
    }

    private void RestoreObjectConnections<TS>(TS t) where TS : T
    {
        var collections = typeof(TS).GetProperties(BindingFlags.Instance|BindingFlags.Public)
            .Where(c => c.PropertyType.IsGenericType
                        && c.PropertyType.GetGenericTypeDefinition() == typeof(AssociationCollection<>));

        foreach (var propertyInfo in collections)
        {
            var collection = (IAssociationCollection)propertyInfo.GetMethod.Invoke(t, null);
            collection.RestoreObjects(t, _serviceProvider);
        }

        var propIdReferences = typeof(TS).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name.StartsWith("Id") && p.Name != ("Id"));
            
        foreach (var propertyInfo in propIdReferences)
        {
            var referenceId = (Guid?)propertyInfo.GetMethod.Invoke(t, null);
            if(referenceId == null) continue;

            var referenceName = propertyInfo.Name.Remove(0, 2);
            var referenceObjProp =  typeof(TS).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(p => p.Name == referenceName);

            var storage = _serviceProvider.FindStorage(referenceObjProp.PropertyType);
            var instans = storage.GetById(referenceId.Value);
            referenceObjProp.SetMethod.Invoke(t,  new [] {instans});
        }
        
        // var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        // if (storageAction is not null)
        //     storageAction?.OnRestore(t); 
        // else
        //     _serviceProvider.GetService<IStorageAction<T>>()?.OnRestore(t);
    }
    
    public IEnumerable<T> FindBy(Func<T, bool> predicate) => _list.Where(predicate);
    //return shallow copy with/without associationCollection?
    //if with -> make sure associationCollection is not editable
    //else -> use reflection only for fields?
    public int Count => _list.Count;
    
}