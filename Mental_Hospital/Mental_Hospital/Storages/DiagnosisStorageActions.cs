using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class DiagnosisStorageActions : IStorageAction<Diagnosis>
{
    private readonly Storage<Patient> _patientStorage;


    public DiagnosisStorageActions(Storage<Patient> personStorage)
    {
        _patientStorage = personStorage;
    }
    public void OnDelete(Diagnosis item)
    {
        var patient = item.Patient;
        item.Patient = null;
        patient.Diagnoses.Remove(item);
    }

    public void OnAdd(Diagnosis item)
    {
        if(item.Patient is not null)
            item.Patient.Diagnoses.Add(item);
        else
        {
            foreach (var person in _patientStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdPatient))
                {
                    item.Patient = person;
                    person.Diagnoses.Add(item);
                }
            }
        }
    }
}