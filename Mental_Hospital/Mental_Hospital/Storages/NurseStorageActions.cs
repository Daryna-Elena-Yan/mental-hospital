﻿using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class NurseStorageActions:IStorageAction<Nurse>
{
    private readonly Storage<Person> _personStorage;
    private readonly Storage<Room> _roomStorage;

    public NurseStorageActions(Storage<Person> personStorage,Storage<Room> roomStorage)
    {
        _personStorage = personStorage;
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
        if (item.Supervisor != null)
        {
            item.Supervisor.Subordinates.Add(item);
        }
    }

    public void OnRestore(Nurse item)
    {
        var foundPerson = _personStorage.FindBy(x => x.IdPerson.Equals(item.IdSupervisor)).FirstOrDefault();
        if (foundPerson is not null)
        {
            var employee = foundPerson as Employee;
              item.Supervisor = employee;
              employee.Subordinates.Add(item);
        }
          
           
        foreach (var rid in item.IdsRooms)
        {
            var room = _roomStorage.FindBy(x => x.IdRoom == rid).FirstOrDefault();
                item.Rooms.Add(room);
                room.Nurses.Add(item);
        }
    }
}
    