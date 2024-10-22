namespace Mental_Hospital.Storages;

public interface IStorageAction<T>
{
    public void OnDelete(T item);
}