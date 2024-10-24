using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital_Tests;

public class Tests
{
    private PersonFactory _personFactory;
    private Storage<Person> _personStorage;
    private DiagnosisFactory _diagnosisFactory;
    private Storage<Diagnosis> _diagnosisStorage;
    private AppointmentFactory _appointmentFactory;
    private Storage<Appointment> _appointmentStorage;
    private EquipmentFactory _equipmentFactory;
    private Storage<Equipment> _equipmentStorage;
    private RoomPatientFactory _roomPatientFactory;
    private Storage<RoomPatient> _roomPatientStorage;
    private RoomFactory _roomFactory;
    private Storage<Room> _roomStorage;
    private PrescriptionFactory _prescriptionFactory;
    private Storage<Prescription> _prescriptionStorage;

    [SetUp]
    public void Setup()
    {
        //Initiallizing DI container
        var services = new ServiceCollection();
        services.MentalHospitalSetup();
        var provider = services.BuildServiceProvider();

        //getting of services instances for tests
        _personFactory = provider.GetRequiredService<PersonFactory>();
        _diagnosisFactory = provider.GetRequiredService<DiagnosisFactory>();
        _appointmentFactory = provider.GetRequiredService<AppointmentFactory>();
        _equipmentFactory = provider.GetRequiredService<EquipmentFactory>();
        _roomFactory = provider.GetRequiredService<RoomFactory>();
        _roomPatientFactory = provider.GetRequiredService<RoomPatientFactory>();
        _prescriptionFactory = provider.GetRequiredService<PrescriptionFactory>();
        
        _personStorage = provider.GetRequiredService<Storage<Person>>();
        _diagnosisStorage = provider.GetRequiredService<Storage<Diagnosis>>();
        _appointmentStorage = provider.GetRequiredService<Storage<Appointment>>();
        _equipmentStorage = provider.GetRequiredService<Storage<Equipment>>();
        _roomStorage = provider.GetRequiredService<Storage<Room>>();
        _roomPatientStorage = provider.GetRequiredService<Storage<RoomPatient>>();
        _prescriptionStorage = provider.GetRequiredService<Storage<Prescription>>();
    }

    [Test]
    public void PersonStorageRegisterTest()
    {
        _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void AddNewDiagnosisTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.Count, Is.EqualTo(1));
        Assert.That(patient.Diagnoses.Where(x => x == diagnosis).Count, Is.EqualTo(1));
    }
    
    [Test]
    public void DeleteDiagnosisTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        
        _diagnosisStorage.Delete(diagnosis);
        
        Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
        Assert.That(patient.Diagnoses.Where(x => x == diagnosis).Count, Is.EqualTo(0));
    }
    
    [Test]
    public void PatientStorageDeleteWithDiagnosesTest()
    {
      var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
      var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
          (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
      
      Assert.That(_personStorage.Count, Is.EqualTo(1));
      Assert.That(_diagnosisStorage.Count, Is.EqualTo(1));
      Assert.That(patient.Diagnoses.Count, Is.EqualTo(1));
      Assert.That(diagnosis.Patient, !Is.Null);
      
      _personStorage.Delete(patient);
      
      Assert.That(_personStorage.Count, Is.EqualTo(0));
      Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void RoomStorageRegisterTest()
    {
        _roomFactory.CreateNewRoom(3);
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void RoomStorageDeleteTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        var roomPatient = _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
      
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));
        
        Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        
        _roomStorage.Delete(room);
        
        Assert.That(_roomStorage.Count, Is.EqualTo(0));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(0));
        Assert.That(_personStorage.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void RoomPatientStorageRegisterTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
        
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));
        
        Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void RoomPatientStorageDeleteTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        var roomPatient = _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);

        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));
        
        Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        
        _roomPatientStorage.Delete(roomPatient);
        
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(0));
        
        Assert.That(patient.RoomPatients.Count, Is.EqualTo(0));
        Assert.That(room.RoomPatients.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void PatientStorageDeleteWithRoomPatientTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
      
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));
        
        Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        
        _personStorage.Delete(patient);
      
        Assert.That(_personStorage.Count, Is.EqualTo(0));
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage.Count, Is.EqualTo(0));
        
        Assert.That(room.RoomPatients.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void AppointmentStorageRegisterTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        Assert.That(appointment.Patient, !Is.Null);
        Assert.That(appointment.Therapist, !Is.Null);
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(patient.Appointments.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void AppointmentStorageDeleteTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        
        Assert.That(appointment.Patient, !Is.Null);
        Assert.That(appointment.Therapist, !Is.Null);
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        
        _appointmentStorage.Delete(appointment);
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(0));
        Assert.That(therapist.Appointments.Count, Is.EqualTo(0));
        Assert.That(patient.Appointments.Count, Is.EqualTo(0));
    }
    
    
    [Test]
    public void PatientStorageDeleteWithAppointmentTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", []);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        
        Assert.That(appointment.Patient, !Is.Null);
        Assert.That(appointment.Therapist, !Is.Null);
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        
        _personStorage.Delete(patient);
        
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(appointment.Patient, Is.Null);
    }
    
    [Test]
    public void PrescriptionStorageRegisterTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, "do sth");
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));
        
        Assert.That(prescription.Appointment, !Is.Null);
        Assert.That(appointment.Patient, !Is.Null);
        Assert.That(appointment.Therapist, !Is.Null);
        Assert.That(appointment.Prescriptions.Count, Is.EqualTo(1));
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        
        Assert.IsTrue(appointment.Prescriptions.ContainsKey(prescription.GetHashCode()));
        Assert.IsTrue(appointment.Prescriptions[prescription.GetHashCode()].Equals(prescription));
    }
    
    [Test]
    public void PrescriptionStorageDeleteTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, "do sth");
        var prescriptionAnother = _prescriptionFactory.CreateNewPrescription(appointment, "Be not healthy", 12, 0.2m, "not do sth");
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        Assert.That(_prescriptionStorage.Count, Is.EqualTo(2));
        
        Assert.That(prescription.Appointment, !Is.Null);
        Assert.That(appointment.Patient, !Is.Null);
        Assert.That(appointment.Therapist, !Is.Null);
        Assert.That(appointment.Prescriptions.Count, Is.EqualTo(2));
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        
        Assert.IsTrue(appointment.Prescriptions.ContainsKey(prescription.GetHashCode()));
        Assert.IsTrue(appointment.Prescriptions[prescription.GetHashCode()].Equals(prescription));
        
        _prescriptionStorage.Delete(prescription);
        
        Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        Assert.That(appointment.Prescriptions.Count, Is.EqualTo(1));
        Assert.IsFalse(appointment.Prescriptions.ContainsKey(prescription.GetHashCode()));
    }
    
    [Test]
    public void AppointmentStorageDeleteWithPrescriptionTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, "do sth");
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
        Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));
        
        Assert.That(appointment.Patient, !Is.Null);
        Assert.That(appointment.Therapist, !Is.Null);
        Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
        Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        
        _appointmentStorage.Delete(appointment);
        
        Assert.That(_personStorage.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage.Count, Is.EqualTo(0));
        Assert.That(therapist.Appointments.Count, Is.EqualTo(0));
        Assert.That(patient.Appointments.Count, Is.EqualTo(0));
        Assert.That(prescription.Appointment, Is.Null);
    }
    
    
    [Test]
    public void AllDiagnosisTypeCreationTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        var diagnosis4 = _diagnosisFactory.CreateNewLightMood
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        var diagnosis5 = _diagnosisFactory.CreateNewLightPsychotic
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        var diagnosis1 = _diagnosisFactory.CreateNewSevereAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null, LevelOfDanger.High, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereMood
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null, LevelOfDanger.High, true);
        var diagnosis3 = _diagnosisFactory.CreateNewSeverePsychotic
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null, LevelOfDanger.High, true);

        
        Assert.That(_diagnosisStorage.FindBy(x => x is SevereMood) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is SeverePsychotic) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is SevereAnxiety) .Count, Is.EqualTo(1));
        
        Assert.That(_diagnosisStorage.FindBy(x => x is LightMood) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is LightPsychotic) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is LightAnxiety) .Count, Is.EqualTo(1));
        
    }
    
    [Test]
    public void EquipmentStorageRegisterTest()
    {
        _equipmentFactory.CreateNewEquipment("IV stand", DateTime.Today);
        Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void EquipmentStorageDeleteTest()
    {
        var equipment = _equipmentFactory.CreateNewEquipment("IV stand", DateTime.Today);
        var room = _roomFactory.CreateNewRoom(3);
        
        room.Equipments.Add(equipment);
        equipment.Room = room;
        
        Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
        
        _equipmentStorage.Delete(equipment);
        
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_equipmentStorage.Count, Is.EqualTo(0));
        Assert.That(room.Equipments.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void RoomStorageDeleteWithEquipmentTest()
    {
        var room = _roomFactory.CreateNewRoom(3);
        var equipment = _equipmentFactory.CreateNewEquipment("IV stand", DateTime.Today);
        room.Equipments.Add(equipment);
        equipment.Room = room;
        
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
        Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
        
        _roomStorage.Delete(room);
        
        Assert.That(_roomStorage.Count, Is.EqualTo(0));
        Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
        Assert.That(equipment.Room, Is.Null);
    }

}