using System.Collections;
using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class DiagnosisStorageActions : IStorageAction<Diagnosis>
{
    private readonly Storage<Person> _personStorage;

    public DiagnosisStorageActions(Storage<Person> personStorage)
    {
        this._personStorage = personStorage;
    }
    
    public void OnDelete(Diagnosis item)
    {
        var patient = item.Patient;
        item.Patient = null;
        patient.Diagnoses.Remove(item);
    }

    public void OnAdd(Diagnosis item)
    {
        var patient = item.Patient;
        patient.Diagnoses.Add(item);
    }

    public void OnRestore(Diagnosis item)
    {
        var patient = _personStorage.FindBy(x => x.IdPerson == item.IdPatient).First() as Patient;
        item.Patient = patient!;
        patient!.Diagnoses.Add(item);
    }

}