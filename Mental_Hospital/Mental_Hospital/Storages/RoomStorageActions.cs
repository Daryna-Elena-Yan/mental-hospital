using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class RoomStorageActions : IStorageAction<Room>
{
    private readonly Storage<RoomPatient> _roomPatientStorage;

    public RoomStorageActions(Storage<RoomPatient> roomPatientStorage)
    {
        _roomPatientStorage = roomPatientStorage;
    }
    public void OnDelete(Room room)
    {
        foreach (RoomPatient roomPatient in room.RoomPatients.ToList())
        {
            _roomPatientStorage.Delete(roomPatient);
        }
        
        foreach (Equipment equipment in room.Equipments.ToList())
        {
            equipment.Room = null;
        }
    }

    public void OnAdd(Room room)
    {
    
    }

    public void OnRestore(Room item)
    {
            throw new NotImplementedException();
    }

    public void OnDeserialize(Room item)
    {
        
    }


}