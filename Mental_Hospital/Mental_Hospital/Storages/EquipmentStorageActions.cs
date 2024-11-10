using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class EquipmentStorageActions: IStorageAction<Equipment>
{
    public void OnDelete(Equipment item)
    {
        if(item.Room is not null)
            item.Room.Equipments.Remove(item);
    }

    public void OnAdd(Equipment item)
    {
   
    }

    public void OnRestore(Equipment item)
    {
        
    }
}