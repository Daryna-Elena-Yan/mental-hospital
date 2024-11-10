using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class RoomPatientStorageActions : IStorageAction<RoomPatient>
{
    public void OnDelete(RoomPatient item)
    {
        item.Room.RoomPatients.Remove(item);
        item.Patient.RoomPatients.Remove(item);
    }

    public void OnAdd(RoomPatient item)
    {
        item.Room.RoomPatients.Add(item);
        item.Patient.RoomPatients.Add(item);
    }

    public void OnRestore(RoomPatient item)
    {
       
    }
    
}