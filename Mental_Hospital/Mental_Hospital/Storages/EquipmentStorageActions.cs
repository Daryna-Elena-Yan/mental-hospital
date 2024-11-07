using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class EquipmentStorageActions: IStorageAction<Equipment>
{
    private readonly Storage<Room> _roomStorage;


    public EquipmentStorageActions(Storage<Room> roomStorage)
    {
        _roomStorage = roomStorage;
    }
    public void OnDelete(Equipment item)
    {
        if(item.Room is not null)
            item.Room.Equipments.Remove(item);
    }

    public void OnAdd(Equipment item)
    {
        if(item.Room is not null)
            item.Room.Equipments.Add(item);
        else
        {
            foreach (var room in _roomStorage.GetList())
            {
                if (room.IdRoom.Equals(item.IdRoom))
                {
                    item.Room = room;
                    room.Equipments.Add(item);
                }
            }
        }
    }
}