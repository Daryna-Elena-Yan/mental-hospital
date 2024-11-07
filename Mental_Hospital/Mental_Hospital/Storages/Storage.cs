using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Storages;

public class Storage<T>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<T> _list= [];

    public Storage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public List<T> GetList()
    {
        return _list;
    }
    public void RegisterNew<TS>(TS t) where TS:T
    {
        var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        if (storageAction is not null)
            storageAction?.OnAdd(t);
        else
            _serviceProvider.GetService<IStorageAction<T>>()?.OnAdd(t);
       
        
        // if sp wont find service registration, nothong will be called
        _list.Add(t);
    }

    public void Link<TS>(TS t) where TS : T
    {
        var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        if (storageAction is not null)
            storageAction?.OnAdd(t);
        else
            _serviceProvider.GetService<IStorageAction<T>>()?.OnAdd(t);
    }
    public void Add<TS>(TS t) where TS : T
    {
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


    public IEnumerable<T> FindBy(Func<T, bool> predicate) => _list.Where(predicate);
    public int Count => _list.Count;
    
}