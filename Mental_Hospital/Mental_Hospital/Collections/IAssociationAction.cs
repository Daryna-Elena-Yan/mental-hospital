using Mental_Hospital.Models;

namespace Mental_Hospital.Collections;

public interface IAssociationAction<T> where T : IEntity
{
    public void OnAdd(T item);
    public void OnDelete(T item);
}