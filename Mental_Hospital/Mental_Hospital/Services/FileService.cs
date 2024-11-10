using System.Collections;
using System.Text.Json;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Services;

public class FileService
{
    private readonly Dictionary<Type, IStorage> _storages = new();
 

    public FileService(IEnumerable<IStorage> storages)
    {
        foreach (var stor in storages)
        {
            _storages.Add(stor.GetType().GenericTypeArguments[0], stor);
        }
    }

    public Storage<T> GetStorage<T>() where T : IEntity
    {
        return (_storages[typeof(T)] as Storage<T>)!;
    }
    public void Serialize()
    {
        
        //TODO mark storage type in resulting file
       /* 
        File.WriteAllText("data.json",JsonSerializer.Serialize(data));*/
    }
    public  void Deserialize()
    {
        /*var data =JsonSerializer.Deserialize<AllData>(GetString() ?? string.Empty);
        foreach (var item in data.Rooms)
        {
            var absent = _roomStorage.GetList().All(a => !a.IdRoom.Equals(item.IdRoom));
            if (absent)
                _roomStorage.Add(item);
        }foreach (var item in data.Equipments)
        {
            var absent = _equipmentStorage.GetList().All(a => !a.IdEquipment.Equals(item.IdEquipment));
            if(absent)
                _equipmentStorage.RegisterNew(item);
        }foreach (var item in data.Appointments)
        {
            var absent = _appointmentStorage.GetList().All(a => 
                !a.IdAppointment.Equals(item.IdAppointment));
            if(absent)
                _appointmentStorage.RegisterNew(item);
        }foreach (var item in data.Patients)
        {
            var absent = _patientStorage.GetList().All(a => !a.IdPerson.Equals(item.IdPerson));
            if(absent)
                _patientStorage.RegisterNew(item);
        }foreach (var item in data.Nurses)
        {
            var absent = _nurseStorage.GetList().All(a => !a.IdPerson.Equals(item.IdPerson));
            if(absent)
                _nurseStorage.RegisterNew(item);
        }foreach (var item in data.Therapists)
        {
            var absent = _therapistStorage.GetList().All(a => !a.IdPerson.Equals(item.IdPerson));
            if(absent)
                _therapistStorage.RegisterNew(item);
        }foreach (var item in data.RoomPatients)
        {
            var absent = _roomPatientStorage.GetList().All(a => !a.IdRoom.Equals(item.IdRoom)&&
                                                                !a.IdPatient.Equals(item.IdPatient)&&
                                                                !a.DatePlaced.Equals(item.DatePlaced));
            if(absent)
                _roomPatientStorage.RegisterNew(item);
        }foreach (var item in data.Diagnoses)
        {
            var absent = _diagnosisStorage.GetList().All(a => !a.IdDisorder.Equals(item.IdDisorder));
            if(absent)
                _diagnosisStorage.RegisterNew(item);
        }foreach (var item in data.Prescriptions)
        {
            var absent = _prescriptionStorage.GetList().All(a => !a.IdPrescription.Equals(item.IdPrescription));
            if(absent)
                _prescriptionStorage.RegisterNew(item);
        }

        foreach (var e in _equipmentStorage.GetList())
        {
            _equipmentStorage.Link(e);
        }
        foreach (var e in _roomStorage.GetList())
        {
            _roomStorage.Link(e);
        }
        foreach (var e in _roomPatientStorage.GetList())
        {
            _roomPatientStorage.Link(e);
        }
        foreach (var e in _appointmentStorage.GetList())
        {
            _appointmentStorage.Link(e);
        }
        foreach (var e in _therapistStorage.GetList())
        {
            _therapistStorage.Link(e);
        }foreach (var e in _patientStorage.GetList())
        {
            _patientStorage.Link(e);
        }foreach (var e in _nurseStorage.GetList())
        {
            _nurseStorage.Link(e);
        }
        foreach (var e in _diagnosisStorage.GetList())
        {
            _diagnosisStorage.Link(e);
        }
        foreach (var e in _prescriptionStorage.GetList())
        {
            _prescriptionStorage.Link(e);
        }
        
        */
    }

    public static string? GetString()
    {
        return File.ReadAllText("data.json");
    }
}

