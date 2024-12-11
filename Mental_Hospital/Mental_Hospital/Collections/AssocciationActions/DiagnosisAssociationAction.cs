using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Collections.AssocciationActions;

public class DiagnosisAssociationAction : IAssociationAction<Diagnosis>
{
    private readonly Storage<Diagnosis> _storage;

    public DiagnosisAssociationAction(Storage<Diagnosis> _storage)
    {
        this._storage = _storage;
    }
    public void OnAdd(Diagnosis item)
    {
        //beacause patient is already associated with diagnosis on creation
    }

    public void OnDelete(Diagnosis item)
    {
        _storage.Delete(item);
    }

    public void OnClear(Diagnosis item)
    {
        _storage.Delete(item);
    }
}