using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PatientStorageActions : IStorageAction<Person>
{
    private readonly Storage<Diagnosis> _diagnosis;

    public PatientStorageActions(Storage<Diagnosis> diagnosis)
    {
        _diagnosis = diagnosis;
    }

    public void OnDelete(Person item)
    {
        var diagnoses = _diagnosis
            .FindBy(x => x.Patient == item)
            .ToArray();
        
        foreach (var diag in diagnoses)
        {
            _diagnosis.Delete(diag);
        }
    }

    public void OnAdd(Person item)
    {
        
    }
}