using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Storages;

public class Storage<T>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<T> _list = [];

    public Storage(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void RegisterNew<TS>(TS t) where TS:T
    {
        var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        storageAction?.OnAdd(t);
        _list.Add(t);
    }

    public void Delete<TS>(TS t) where TS:T
    {
        var storageAction = _serviceProvider.GetService<IStorageAction<TS>>();
        storageAction?.OnDelete(t); // if sp wont find service registration, nothong will be called
        _list.Remove(t);
    }


    public IEnumerable<T> FindBy(Func<T, bool> predicate) => _list.Where(predicate);
    public int Count => _list.Count;
    
}