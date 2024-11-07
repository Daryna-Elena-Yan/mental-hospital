using Mental_Hospital.Models;

namespace Mental_Hospital.Storages;

public class RoomPatientStorageActions : IStorageAction<RoomPatient>
{
        private readonly Storage<Room> _roomStorage;
        private readonly Storage<Patient> _patientStorage;


        public RoomPatientStorageActions(Storage<Patient> personStorage,Storage<Room> r)
        {
            _patientStorage = personStorage;
            _roomStorage = r;
        }
    public void OnDelete(RoomPatient item)
    {
        item.Room.RoomPatients.Remove(item);
        item.Patient.RoomPatients.Remove(item);
    }

    public void OnAdd(RoomPatient item)
    {
        if(item.Patient is not null)
            item.Patient.RoomPatients.Add(item);
        else
        {
            foreach (var person in _patientStorage.GetList())
            {
                if (person.IdPerson.Equals(item.IdPatient))
                {
                    item.Patient = person;
                    person.RoomPatients.Add(item);
                }
            }
        }
        if(item.Room is not null)
            item.Room.RoomPatients.Add(item);
        else
        {
            foreach (var room in _roomStorage.GetList())
            {
                if (room.IdRoom.Equals(item.IdRoom))
                {
                    item.Room = room;
                    room.RoomPatients.Add(item);
                }
            }
        }
    }
}