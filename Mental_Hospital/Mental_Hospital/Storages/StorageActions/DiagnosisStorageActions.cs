using System.Collections;
using Mental_Hospital.Collections;
using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class DiagnosisStorageActions : IStorageAction<Diagnosis>
{
    private readonly Storage<Person> _personStorage;
    private readonly IServiceProvider _provider;

    public DiagnosisStorageActions(Storage<Person> personStorage, IServiceProvider _provider)
    {
        this._personStorage = personStorage;
        this._provider = _provider;
    }
    
    public void OnDelete(Diagnosis item)
    {
        var patient = item.Patient;
        item.Patient = null;
        patient?.Diagnoses.Remove(item);
    }

    public void OnAdd(Diagnosis item)
    {
        var patient = item.Patient;
        patient.Diagnoses.Add(item);
    }



}