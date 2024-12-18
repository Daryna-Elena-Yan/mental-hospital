using Mental_Hospital.Models;

namespace Mental_Hospital.Collections;

public interface IAssociationCollection
{
    void RestoreObjects(IEntity parent, IServiceProvider serviceProvider);
}