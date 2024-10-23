using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class EquipmentActions: IStorageAction<Equipment>
{
    public void OnDelete(Equipment item)
    {
        if(item.Room is not null)
            item.Room.Equipments.Remove(item);
    }

    public void OnAdd(Equipment item)
    {
   
    }
}