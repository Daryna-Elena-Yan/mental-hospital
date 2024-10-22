using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class Storage<T>
{
    private List<T> _list = [];

    public void RegisterNew(T t)
    {
        _list.Add(t);
    }
    
    public int Count => _list.Count;
    
}