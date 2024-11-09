using System.Text.Json;
using Mental_Hospital.DTO;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Services;

public class FileService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Storage<Diagnosis> _diagnosisStorage;
    private readonly Storage<Appointment> _appointmentStorage;
    private readonly Storage<Equipment> _equipmentStorage;
    private readonly Storage<Therapist> _therapistStorage;
    private readonly Storage<Patient> _patientStorage;
    private readonly Storage<Nurse> _nurseStorage;
    private readonly Storage<Prescription> _prescriptionStorage;
    private readonly Storage<RoomPatient> _roomPatientStorage;
    private readonly Storage<Room> _roomStorage;

    public FileService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _therapistStorage = serviceProvider.GetService<Storage<Therapist>>();
        _nurseStorage = serviceProvider.GetService<Storage<Nurse>>();
        _patientStorage = serviceProvider.GetService<Storage<Patient>>();
        _diagnosisStorage = serviceProvider.GetService<Storage<Diagnosis>>();
        _appointmentStorage = serviceProvider.GetService<Storage<Appointment>>();
        _equipmentStorage = serviceProvider.GetService<Storage<Equipment>>();
        _prescriptionStorage = serviceProvider.GetService<Storage<Prescription>>();
        _roomStorage = serviceProvider.GetService<Storage<Room>>();
        _roomPatientStorage = serviceProvider.GetService<Storage<RoomPatient>>();
    }
    public  void Serialize()
    {
       /* var data = new AllData
        {
            Patients = _patientStorage.GetList(),
            Therapists = _therapistStorage.GetList(),
            Nurses = _nurseStorage.GetList(),
            Diagnoses = _diagnosisStorage.GetList(),
            Appointments = _appointmentStorage.GetList(),
            Equipments = _equipmentStorage.GetList(),
            Prescriptions = _prescriptionStorage.GetList(),
            Rooms = _roomStorage.GetList(),
            RoomPatients = _roomPatientStorage.GetList()
        };
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

