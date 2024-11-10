using System.Collections;
using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class DiagnosisStorageActions : IStorageAction<Diagnosis>
{
    
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
        throw new NotImplementedException();
    }

    public void OnDeserialize(Diagnosis item)
    {
         
    }
}