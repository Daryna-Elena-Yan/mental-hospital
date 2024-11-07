using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class NurseStorageActions:IStorageAction<Nurse>
{
        private readonly Storage<Room> _roomStorage;
        private readonly Storage<Nurse> _nurseStorage;
        private readonly Storage<Therapist> _therapistStorage;


        public NurseStorageActions(Storage<Room> roomStorage,Storage<Nurse> personStorage,Storage<Therapist> s)
        {
            _nurseStorage = personStorage;
            _therapistStorage = s;
            _roomStorage = roomStorage;
        }
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
        if(item.Supervisor is not null)
            item.Supervisor.Subordinates.Add(item);
        else
        {
            foreach (var person in _therapistStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdSupervisor))
                {
                    item.Supervisor = person;
                    person.Subordinates.Add(item);
                }
            }
            foreach (var person in _nurseStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdSupervisor))
                {
                    item.Supervisor = person;
                    person.Subordinates.Add(item);
                }
            }
        }

        if (item.Rooms.Count > 0)
        {
            foreach (var room in item.Rooms)
            {
                room.Nurses.Add(item);
            }
        }
        else
        {
            foreach (var room in _roomStorage.GetList())
            {
                foreach (var rid in item.IdsRooms)
                {
                    if (room.IdRoom.Equals(rid))
                    {
                        item.Rooms.Add(room);
                        room.Nurses.Add(item);
                    }
                }
                
            }
        }
    }
}