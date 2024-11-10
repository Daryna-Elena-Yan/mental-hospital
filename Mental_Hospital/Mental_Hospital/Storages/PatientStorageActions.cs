using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class PatientStorageActions : IStorageAction<Patient>
{
    private readonly Storage<Diagnosis> _diagnosisStorage;
    private readonly Storage<RoomPatient> _roomPatientStorage;

    public PatientStorageActions(Storage<Diagnosis> diagnosisStorage, Storage<RoomPatient> roomPatientStorage)
    {
        _diagnosisStorage = diagnosisStorage;
        _roomPatientStorage = roomPatientStorage;
    }

    public void OnDelete(Patient patient)
    {
        foreach (Diagnosis diagnosis in patient.Diagnoses.ToList())
        {
            _diagnosisStorage.Delete(diagnosis);
        }
        
        foreach (RoomPatient roomPatient in patient.RoomPatients.ToList())
        {
            _roomPatientStorage.Delete(roomPatient);
        }
        
        foreach (Appointment appointment in patient.Appointments.ToList())
        {
            appointment.Patient = null;
        }
        
    }

    public void OnAdd(Patient patient)
    {
        
    }

    public void OnRestore(Patient item)
    {
        
    }


}