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
    
    public void RegisterNew(T t)
    {
        _list.Add(t);
    }

    public void Delete(T t)
    {
        _serviceProvider.GetService<IStorageAction<T>>()?.OnDelete(t); // if sp wont find service registration, nothong will be called
        _list.Remove(t);
    }


    public IEnumerable<T> FindBy(Func<T, bool> predicate) => _list.Where(predicate);
    public int Count => _list.Count;
    
}