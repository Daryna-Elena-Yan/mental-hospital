using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class DiagnosisActions : IStorageAction<Diagnosis>
{
    public void OnDelete(Diagnosis item)
    {
        
    }

    public void OnAdd(Diagnosis item)
    {
        item.Patient.Diagnoses.Add(item);
    }
}