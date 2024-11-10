using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class NurseStorageActions:IStorageAction<Nurse>
{
    public void OnDelete(Nurse item)
    {
        foreach (var room in item.Rooms.ToList())
        {
            room.Nurses.Remove(item);
        }
        item.Supervisor?.Subordinates.Remove(item);
        foreach (var employee in item.Subordinates.ToList())
        {
            employee.Supervisor = null;
        }
    }

    public void OnAdd(Nurse item)
    {
        if (item.Supervisor != null)
        {
            item.Supervisor.Subordinates.Add(item);
        }
    }

    public void OnRestore(Nurse item)
    {
        throw new NotImplementedException();
    }
    
    
}