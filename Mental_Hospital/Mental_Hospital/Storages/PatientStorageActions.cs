using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PatientStorageActions : IStorageAction<Person>
{
    private readonly Storage<Diagnosis> _diagnosisStorage;
    private readonly Storage<RoomPatient> _roomPatientStorage;

    public PatientStorageActions(Storage<Diagnosis> diagnosisStorage, Storage<RoomPatient> roomPatientStorage)
    {
        _diagnosisStorage = diagnosisStorage;
        _roomPatientStorage = roomPatientStorage;
    }

    public void OnDelete(Person item)
    {
        if (item is Patient)
        {
            Patient patient = (Patient) item;
            
            /*var diagnoses = _diagnosisStorage
                        .FindBy(x => x.Patient == patient)
                        .ToArray();
                    
            foreach (var diag in diagnoses)
            {
                _diagnosisStorage.Delete(diag);
            }*/
            
            foreach (Diagnosis diagnosis in patient.Diagnoses)
            {
                _diagnosisStorage.Delete(diagnosis);
            }
            
            foreach (RoomPatient roomPatient in patient.RoomPatients)
            {
                _roomPatientStorage.Delete(roomPatient);
            }
            
            foreach (Appointment appointment in patient.Appointments)
            {
                appointment.Patient = null;
            }
        }
    }

    public void OnAdd(Person item)
    {
        
    }
}