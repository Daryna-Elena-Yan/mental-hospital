using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PatientStorageActions : IStorageAction<Person>
{
    private readonly Storage<Diagnosis> _diagnosis;
    private readonly Storage<PatientDiagnosis> _patientDiagnosis;

    public PatientStorageActions(
        Storage<Diagnosis> diagnosis, Storage<PatientDiagnosis> patientDiagnosis)
    {
        _diagnosis = diagnosis;
        _patientDiagnosis = patientDiagnosis;
    }

    public void OnDelete(Person item)
    {
        var diagnoses = _patientDiagnosis
            .FindBy(x => x.Patient == item)
            .Select(x => x.Diagnosis).ToArray();
        
        foreach (var diag in diagnoses)
        {
            _diagnosis.Delete(diag);
        }

        var pds = _patientDiagnosis
            .FindBy(x => x.Patient == item).ToArray();
        foreach (var pd in pds)
        {
            _patientDiagnosis.Delete(pd);
        }
    }
}