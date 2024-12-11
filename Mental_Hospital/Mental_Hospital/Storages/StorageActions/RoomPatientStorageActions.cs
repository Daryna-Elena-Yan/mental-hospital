using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class RoomPatientStorageActions : IStorageAction<RoomPatient>
{
    private readonly Storage<Room> _roomStorage;
    private readonly Storage<Person> _personStorage;
    public RoomPatientStorageActions(Storage<Room> roomStorage, Storage<Person> personStorage)
    {
        _roomStorage = roomStorage;
        _personStorage = personStorage;
    }
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
        var room = _roomStorage.FindBy(x => x.Id == item.IdRoom).First();
        item.Room = room;
        room.RoomPatients.Add(item);
        
        var patient = _personStorage.FindBy(x => x.Id == item.IdPatient).First() as Patient;
        item.Patient = patient!;
        patient!.RoomPatients.Add(item);
    }
    
}