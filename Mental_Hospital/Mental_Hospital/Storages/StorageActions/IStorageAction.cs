using System.Collections;

namespace Mental_Hospital.Storages;

public interface IStorageAction<T>
{
    public void OnDelete(T item);
    public void OnAdd(T item);
    void OnRestore(T item); 
}