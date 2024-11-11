using System.Globalization;
using FluentValidation;
using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Services;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mental_Hospital_Tests;

public class Tests
{
    private readonly string format = "dd/MM/yyyy";
    private readonly CultureInfo culture = CultureInfo.InvariantCulture;
    private PersonFactory _personFactory;
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
    private StorageManager _storageManager;
    private Storage<Person> _personStorage;

    [SetUp]
    public void Setup()
    {
        // Initialize DI container
        var services = new ServiceCollection();
        services.MentalHospitalSetup();
        var provider = services.BuildServiceProvider();

        // Get service instances for tests
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

        _storageManager = provider.GetRequiredService<StorageManager>();
    }

    [Test]
    public void PersonStorageRegisterTest()
    {
        _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void TherapistCreationAndAttributesTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Today,"korobkaaaaaa", ["s"]);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(therapist1.Name, Is.EqualTo("frst"));
            Assert.That(therapist1.Bonus, Is.EqualTo(0));
            Assert.That(therapist1.Address, Is.EqualTo("korobkaaaaaa"));
            Assert.That(therapist1.Surname, Is.EqualTo("frstovich"));
            Assert.That(therapist1.DateHired, Is.EqualTo(DateTime.Today));
            Assert.That(therapist1.Supervisor, Is.Null);
            Assert.That(therapist1.DateOfBirth, Is.EqualTo(DateTime.Today));
            List<string> l = ["s"];
            Assert.That(therapist1.Qualifications, Is.EqualTo(l));
            Assert.That(therapist1.OvertimePerMonth, Is.EqualTo(0));
        });
    }
    [Test]
    public void TherapistStaticDerivedTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaaa", ["d"]);
        therapist1.Bonus = 20;
        therapist1.OvertimePerMonth = 3;
        therapist1.RecalculateSalary();
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(therapist1.Salary, Is.EqualTo(10230));
        });
    }

    [Test]
    public void NurseCreateTest()
    {
        var nurse = _personFactory.CreateNewNurse(null, "aaa", "AAA", DateTime.Today, "korobochka");
        Assert.That(_personStorage.Count,Is.EqualTo(1));
        var room = _roomFactory.CreateNewRoom(3);
    }

    [Test]
    public void NurseDeleteTest()
    {
        var nurse = _personFactory.CreateNewNurse(null, "aaa", "AAA", DateTime.Today, "korobochka");
        Assert.That(_personStorage.Count,Is.EqualTo(1));
        _personStorage.Delete(nurse);
        Assert.That(_personStorage.Count,Is.EqualTo(0));
    }

    
    [Test]
    public void NurseStaticDerivedTest()
    {
        var nurse =  _personFactory.CreateNewNurse(null, "frst", "frstovich", DateTime.Now,"korobkaaaaaaaaaaaaa");
        nurse.Bonus = 20;
        nurse.OvertimePerMonth = 3;
        nurse.RecalculateSalary();
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(nurse.Salary, Is.EqualTo(6170));
        });
    }
    [Test]
    public void TherapistDeletionTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaaa", ["s"]);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        _personStorage.Delete(therapist1);
        Assert.That(_personStorage.Count, Is.EqualTo(0));
}
    [Test]
    public void TherapistWithSupervisorCreationTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaaaa", ["d"]);
        var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", ["d"]);
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates, Has.Count.EqualTo(1));
            Assert.That(therapist2.Supervisor, !Is.Null);
            Assert.That(_personStorage.Count, Is.EqualTo(2));
        });
    }
    [Test]
    public void TherapistWithSupervisorDeletionTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaa", ["s"]);
        var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", ["s"]);
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(1));
            Assert.That(therapist2.Supervisor, !Is.Null);
            Assert.That(_personStorage.Count, Is.EqualTo(2));
        });
        _personStorage.Delete(therapist2);
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(0));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void AddNewDiagnosisTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_diagnosisStorage.Count, Is.EqualTo(1));
            Assert.That(patient.Diagnoses.Where(x => x == diagnosis).Count, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void DeleteDiagnosisTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
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
          (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_diagnosisStorage.Count, Is.EqualTo(1));
            Assert.That(patient.Diagnoses.Count, Is.EqualTo(1));
            Assert.That(diagnosis.Patient, !Is.Null);
        });

        _personStorage.Delete(patient);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(0));
            Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void RoomStorageRegisterTest()
    {
        _roomFactory.CreateNewRoom(3);
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
    }
    [Test]
         public void NurseWithRoomsCreateTest()
         {
             var nurse = _personFactory.CreateNewNurse(null, "aaa", "AAA", DateTime.Today, "korobochka");
             Assert.That(_personStorage.Count,Is.EqualTo(1));
             var room =_roomFactory.CreateNewRoom(3);
             nurse.Rooms.Add(room);
             room.Nurses.Add(nurse);
             Assert.That(nurse.Rooms.Count,Is.EqualTo(1));
             Assert.That(room.Nurses.Count,Is.EqualTo(1));
             _personStorage.Delete(nurse);
             Assert.That(room.Nurses.Count,Is.EqualTo(0));
         }
    
    [Test]
    public void RoomStorageDeleteTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        var roomPatient = _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));

            Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
            Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        });

        _roomStorage.Delete(room);
        Assert.Multiple(() =>
        {
            Assert.That(_roomStorage.Count, Is.EqualTo(0));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(0));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void RoomPatientStorageRegisterTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));

            Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
            Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void RoomPatientStorageDeleteTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        var roomPatient = _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);

        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));

            Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
            Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        });

        _roomPatientStorage.Delete(roomPatient);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(0));

            Assert.That(patient.RoomPatients.Count, Is.EqualTo(0));
            Assert.That(room.RoomPatients.Count, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void PatientStorageDeleteWithRoomPatientTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        
        _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(1));

            Assert.That(patient.RoomPatients.Count, Is.EqualTo(1));
            Assert.That(room.RoomPatients.Count, Is.EqualTo(1));
        });

        _personStorage.Delete(patient);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(0));
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_roomPatientStorage.Count, Is.EqualTo(0));

            Assert.That(room.RoomPatients.Count, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void AppointmentStorageRegisterTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now,
            "very important appointment for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });
    }
    [Test]
    public void TherapistWithSupervisorAndAppointmentCreationTest()
        {
            var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaaaa", ["d"]);
            var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", ["d"]);
            var appointment = _appointmentFactory.CreateNewAppointment(therapist2, null, DateTime.Now, 
                "very important appointment for your live");
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(1));
            Assert.That(therapist2.Supervisor, !Is.Null);
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist2.Appointments.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(2));
        });
    }
    
    [Test]
    public void AppointmentStorageDeleteTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now,
            "very important appointment for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_storageManager.GetStorage<Person>().Count, Is.EqualTo(2));
            Assert.That(_storageManager.GetStorage<Appointment>().Count, Is.EqualTo(1));

            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });

        _appointmentStorage.Delete(appointment);
        Assert.Multiple(() =>
        {
            Assert.That(_storageManager.GetStorage<Person>().Count, Is.EqualTo(2));
            Assert.That(_storageManager.GetStorage<Appointment>().Count, Is.EqualTo(0));
           
            Assert.That(therapist.Appointments.Count, Is.EqualTo(0));
            Assert.That(patient.Appointments.Count, Is.EqualTo(0));
        });
    }
    [Test]
        public void TherapistWithSupervisorAndAppointmentDeletionTest()
        {
            var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobochkaka", ["d"]);
            var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", ["d"]);
            var appointment = _appointmentFactory.CreateNewAppointment(therapist2, null, DateTime.Now, 
                "very important appointment for your live");
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(1));
            Assert.That(therapist2.Supervisor, !Is.Null);
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(therapist2.Appointments.Count, Is.EqualTo(1));
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(_personStorage.Count, Is.EqualTo(2));
        });
        _personStorage.Delete(therapist2);
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(0));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void PatientStorageDeleteWithAppointmentTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now,
            "very important appointment for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));

            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });

        _personStorage.Delete(patient);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(appointment.Patient, Is.Null);
        });
    }
    
    [Test]
    public void PrescriptionStorageRegisterTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, 
            "very important prescription for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));

            Assert.That(prescription.Appointment, !Is.Null);
            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(appointment.Prescriptions.Count, Is.EqualTo(1));
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });

        Assert.IsTrue(appointment.Prescriptions.ContainsKey(prescription.GetHashCode()));
        Assert.IsTrue(appointment.Prescriptions[prescription.GetHashCode()].Equals(prescription));
    }
    
    [Test]
    public void PrescriptionStorageDeleteTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, 
            "very important prescription for your live");
        var prescriptionAnother = _prescriptionFactory.CreateNewPrescription(appointment, "Be not healthy", 12, 0.2m, 
            "very important prescription for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(_prescriptionStorage.Count, Is.EqualTo(2));

            Assert.That(prescription.Appointment, !Is.Null);
            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(appointment.Prescriptions.Count, Is.EqualTo(2));
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });

        Assert.IsTrue(appointment.Prescriptions.ContainsKey(prescription.GetHashCode()));
        Assert.IsTrue(appointment.Prescriptions[prescription.GetHashCode()].Equals(prescription));
        
        _prescriptionStorage.Delete(prescription);
        Assert.Multiple(() =>
        {
            Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(appointment.Prescriptions.Count, Is.EqualTo(1));
        });
        Assert.IsFalse(appointment.Prescriptions.ContainsKey(prescription.GetHashCode()));
    }
    
    [Test]
            public void TherapistWithSupervisorAndAppointmentAndPrescriptionDeletionTest()
            {
                var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaaaaaa", ["d"]);
                var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", ["D"]);
                var appointment = _appointmentFactory.CreateNewAppointment(therapist2, null, DateTime.Now, 
                    "very important appointment for your live"); 
                var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "stay strong", 30, 0.02m,
                    "very important prescription for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist2.Appointments.Count, Is.EqualTo(1));
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(1));
            Assert.That(therapist2.Supervisor, !Is.Null);
        });
        _personStorage.Delete(therapist2);
        Assert.Multiple(() =>
        {
            Assert.That(therapist1.Subordinates.Count, Is.EqualTo(0));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(0));
            Assert.That(prescription.Appointment, Is.Null);
            Assert.That(_personStorage.Count, Is.EqualTo(1));
        });
    }
    [Test]
    public void AppointmentStorageDeleteWithPrescriptionTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m,
            "very important prescription for your live");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));

            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });

        _appointmentStorage.Delete(appointment);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(2));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(0));
            Assert.That(therapist.Appointments.Count, Is.EqualTo(0));
            Assert.That(patient.Appointments.Count, Is.EqualTo(0));
            Assert.That(prescription.Appointment, Is.Null);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_equipmentStorage.Count, Is.EqualTo(0));
            Assert.That(room.Equipments.Count, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void RoomStorageDeleteWithEquipmentTest()
    {
        var room = _roomFactory.CreateNewRoom(3);
        var equipment = _equipmentFactory.CreateNewEquipment("IV stand", DateTime.Today);
        room.Equipments.Add(equipment);
        equipment.Room = room;
        Assert.Multiple(() =>
        {
            Assert.That(_roomStorage.Count, Is.EqualTo(1));
            Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
        });

        _roomStorage.Delete(room);
        Assert.Multiple(() =>
        {
            Assert.That(_roomStorage.Count, Is.EqualTo(0));
            Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
            Assert.That(equipment.Room, Is.Null);
        });
    }
    
    [Test]
    public void PatientEmptyNameAttributeValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
            
    }
    
    [Test]
    public void PatientEmptySurnameAttributeValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
            
    }
    [Test]
    public void PatientDeathEarlierThatBirthAttributeValidationTest()
    {
        var death =  DateTime.ParseExact("25/12/1999",format,culture);
        var birth = DateTime.ParseExact("25/12/2002",format,culture);

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", birth, "Melbourne, Australia",
            "cases of selfharm in the past", death));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Date of healing cannot be earlier that date of Diagnosing.") , Is.EqualTo(1));
            
    }
    
    [Test]
    public void PatientEmptyAddressAttributeValidationTest()
    {
        var birth = DateTime.ParseExact("25/12/2002",format,culture);

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", birth, "",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
            
    }
    
    [Test]
    public void PatientLongAddressAttributeValidationTest()
    {
        var birth = DateTime.ParseExact("25/12/2002",format,culture);

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", birth, "Austrlia is a very nice country but there are spiders and insect but i think that kangoroos are cute",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
            
    }

    [Test]
    public void PatientNullDateBirthAttributeValidationTest()
    {

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.MinValue, 
            "Melbourne, Australia",
            "cases of selfharm in the past", null));

        Assert.That(ex.Errors.Count(), Is.EqualTo(1));
        Assert.That(
            ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth"),
            Is.EqualTo(1));

    }

    [Test]
    public void DiagnosisEmptyNameAttributeValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        var ex = Assert.Throws<ValidationException>(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "",
            "cases of anexity related to the past",
            new[] { "swap positions" }, DateTime.Now, DateTime.Now.AddDays(1), true));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify name of the Diagnosis.") , Is.EqualTo(1));
            
    }
    
    [Test]
    public void DiagnosisShortDescriptionAttributeValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        var ex = Assert.Throws<ValidationException>(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
            "cases",
            new[] { "swap positions" }, DateTime.Now, DateTime.Now.AddDays(1), true));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void DiagnosisLongDescriptionAttributeValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        var ex = Assert.Throws<ValidationException>(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
            //there is more than 500 symbols below
            "Major Depressive Disorder is characterized by a persistent feeling of sadness or a lack of interest in external activities. Patients exhibit symptoms such as significant weight loss or gain, insomnia or excessive sleeping, fatigue, feelings of worthlessness or excessive guilt, diminished ability to think or concentrate, and recurrent thoughts of death or suicide. These symptoms must be present for at least two weeks and cause significant impairment in social, occupational, or other important areas of functioning. Diagnosis is based on clinical evaluation and adherence to DSM-5 criteria. 501 symbol ahahaha",
            new[] { "swap positions" }, DateTime.Now, DateTime.Now.AddDays(1), true));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void DiagnosisHealingDateBeforeDiagnosisDateAttributeValidationTest()
    {
        var healing = DateTime.ParseExact("25/12/1999", format, culture);
        var diag = DateTime.ParseExact("25/12/2002", format, culture);
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        var ex = Assert.Throws<ValidationException>(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
            "cases of anexity related to the past",
            new[] { "swap positions" }, diag, healing, true));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Date of healing cannot be earlier that date of Diagnosing.") , Is.EqualTo(1));
    }
    
    
    [Test]
    public void DiagnosisHealingIsNullAttributeValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        Assert.DoesNotThrow(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
            "cases of anexity related to the past",
            new[] { "swap positions" }, DateTime.Now.AddDays(1), null, true));
    }
    
    [Test]
    public void DiagnosisPatientIncorrectAttributeValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
        {
            var patient = _personFactory.CreateNewPatient("", "Piastri", DateTime.Now, "Melbourne, Australia",
                "cases of selfharm in the past", null);
            _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
                "cases of anexity related to the past",
                new[] { "swap positions" }, DateTime.Now, DateTime.Now.AddDays(1), true);
        });
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void AppointmentNullDateOfAppointmentAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var ex = Assert.Throws<ValidationException>(() => _appointmentFactory.CreateNewAppointment(therapist, patient,DateTime.MinValue, 
            "very important appointment for your live"));

        Assert.That(ex.Errors.Count(), Is.EqualTo(1));
        Assert.That(
            ex.Errors.Count(x => x.ErrorMessage == "Specify date of appointment."),
            Is.EqualTo(1));

    }
    
    [Test]
    public void AppointmentShortDescriptionAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["g"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var ex = Assert.Throws<ValidationException>(() =>
            _appointmentFactory.CreateNewAppointment(therapist, patient,DateTime.Now, 
            "too short"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void AppointmentLongDescriptionAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var ex = Assert.Throws<ValidationException>(() =>
            _appointmentFactory.CreateNewAppointment(therapist, patient,DateTime.Now, 
                "The patient attended today’s session reporting ongoing symptoms of anxiety and depressive moods, which have worsened over the past month. They described feeling overwhelmed with work stress and difficulty sleeping. The patient shared a history of generalized anxiety disorder and mild depression, both of which have been managed with therapy and medication. During the session, cognitive-behavioral therapy techniques were used to address negative thought patterns. A follow-up appointment is scheduled in two weeks to reassess progress and adjust treatment as necessary."));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void AppointmentTherapistDoesNotExistValidationTest()
    {
        var therapist = new Therapist();
        therapist.Supervisor = null;
        therapist.Name = "Charles";
        therapist.Surname = "Leclerc";
        therapist.DateOfBirth = DateTime.Now;
        therapist.Address = "Baker Street, 221B";
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);

        var ex = Assert.Throws<ValidationException>(() =>
            _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now,
                "very important appointment for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Therapist does not exist.") , Is.EqualTo(1));
    }
    
    [Test]
    public void AppointmentNullPatientValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);

        Assert.DoesNotThrow(() => _appointmentFactory.CreateNewAppointment(therapist, null, DateTime.Now,
                "very important appointment for your live"));
    }
    
    [Test]
    public void AppointmentPatientDoesNotExistValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient = new Patient();
        patient.Name = "Charles";
        patient.Surname = "Leclerc";
        patient.DateOfBirth = DateTime.Now;
        patient.Address = "Baker Street, 221B";
        patient.Anamnesis = "Depression";
        patient.DateOfDeath = null;
        
        var ex = Assert.Throws<ValidationException>(() =>
            _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now,
                "very important appointment for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Patient does not exist.") , Is.EqualTo(1));
    }
    [Test]
    public void PrescriptionNullNameAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            null!, 10, 0.02m, "very important prescription for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Please specify name.") , Is.EqualTo(1));
    }
    
    [Test]
    public void PrescriptionEmptyNameAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            "", 10, 0.02m, "very important prescription for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 1 characters long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void PrescriptionNegativeQuantityAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            "Be healthy", -10, 0.02m, "very important prescription for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Quantity must be greater than or equal to 0.") , Is.EqualTo(1));
    }
    
    [Test]
    public void PrescriptionNegativeDosageAttributeValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            "Be healthy", 10, -0.02m, "very important prescription for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Dosage must be greater than or equal to 0.") , Is.EqualTo(1));
    }

    [Test]
    public void PrescriptionShortDescriptionAttributeValidationTest()
    {
        
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            "Be healthy", 10, 0.02m, "abc"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }

    [Test]
    public void PrescriptionLongDescriptionAttributeValidationTest()
    {
        
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            "Be healthy", 10, 0.02m, "Patient diagnosed with major depressive disorder and anxiety. Prescribed Sertraline 50 mg daily to improve mood and reduce anxiety, starting dose for 2 weeks, then increase to 100 mg if tolerated. Clonazepam 0.25 mg twice a day as needed for acute anxiety, max 0.5 mg per day. Melatonin 3 mg for sleep, taken before bedtime as needed. Patient advised to continue weekly therapy sessions, focus on exercise, maintain sleep hygiene, and follow a healthy diet. Return in 4 weeks for assessment. Contact clinic if severe side effects or mood changes occur."));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void PrescriptionNullAppointmentValidationTest()
    {
        Assert.DoesNotThrow(() =>  _prescriptionFactory.CreateNewPrescription(null, 
            "Be healthy", 10, 0.02m, "very important prescription for your live"));;
    }
    
    [Test]
    public void PrescriptionAppointmentDoesNotExistValidationTest()
    {
        var appointment = new Appointment();
        appointment.Therapist = new Therapist();
        appointment.Patient = null;
        appointment.DateOfAppointment = DateTime.Now;
        appointment.Description = "very important appointment for your live";

        var ex = Assert.Throws<ValidationException>(() => _prescriptionFactory.CreateNewPrescription(appointment, 
            "Be healthy", 10, 0.02m, "very important prescription for your live"));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Appointment does not exist.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EquipmentNullNameAttributeValidationTest()
    {
        
        var ex = Assert.Throws<ValidationException>(() => 
            _equipmentFactory.CreateNewEquipment(null!, DateTime.Today));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Please specify name.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EquipmentEmptyNameAttributeValidationTest()
    {
        
        var ex = Assert.Throws<ValidationException>(() => 
            _equipmentFactory.CreateNewEquipment("", DateTime.Today));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 1 characters long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EquipmentNullExpirationDateAttributeValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => 
            _equipmentFactory.CreateNewEquipment("Some stuff", DateTime.MinValue));

        Assert.That(ex.Errors.Count(), Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of expiration."),
            Is.EqualTo(1));
    }
    
    [Test]
    public void RoomNegativeCapacityAttributeValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _roomFactory.CreateNewRoom(-3));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Capacity must be greater than or equal to 0.") , Is.EqualTo(1));
    }
    
    [Test]
    public void RoomPatientNullDatePlacedAttributeValidationTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var room = _roomFactory.CreateNewRoom(3);
        
        var ex = Assert.Throws<ValidationException>(() => 
            _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.MinValue, null));

        Assert.That(ex.Errors.Count(), Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of being placed."),
            Is.EqualTo(1));
    }
    
    [Test]
    public void RoomPatientDatePlacedAfterDateDischargedAttributeValidationTest()
    {
        var discharged = DateTime.ParseExact("25/12/1999", format, culture);
        var placed = DateTime.ParseExact("25/12/2002", format, culture);
        
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var room = _roomFactory.CreateNewRoom(3);
        
        var ex = Assert.Throws<ValidationException>(() => 
            _roomPatientFactory.CreateNewRoomPatient(room, patient, placed, discharged));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => 
            x.ErrorMessage == "Date of being placed cannot be earlier that date of being discharged.") , Is.EqualTo(1));
    }
    
    [Test]
    public void RoomPatientPatientDoesNotExistValidationTest()
    {
        var patient = new Patient();
        patient.Name = "Charles";
        patient.Surname = "Leclerc";
        patient.DateOfBirth = DateTime.Now;
        patient.Address = "Baker Street, 221B";
        patient.Anamnesis = "Depression";
        
        var room = _roomFactory.CreateNewRoom(3);

        var ex = Assert.Throws<ValidationException>(() =>
            _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null));
        
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Patient does not exist.") , Is.EqualTo(1));
    }
    
    [Test]
    public void RoomPatientRoomDoesNotExistValidationTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);

        var room = new Room();
        room.Capacity = 3;

        var ex = Assert.Throws<ValidationException>(() =>
            _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null));
        
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Room does not exist.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeNameTest()
    {
        var ex = Assert.Throws<ValidationException>(() => 
            _personFactory.CreateNewNurse(null, "1", "helpovich", DateTime.Now, "korobochka_lesnaya"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeSurnameTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, "1", "2", DateTime.Now, "korobochka"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(2));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeBDayTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, "1", "2",DateTime.MinValue, "korobochka"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(3));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeAddressTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, "1", "2",DateTime.MinValue, "korobka"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(4));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
    }
    
    [Test]
    public void TherapistQualificationsTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewTherapist(null, "1", "2",DateTime.MinValue, "korobka",[]));
        Assert.That(ex.Errors.Count() , Is.EqualTo(5));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Therapist must have at least one qualification.") , Is.EqualTo(1));
    }

    [Test]
    public void FakeSupervisorTest()
    {
        var fake = new Therapist();
        fake.IdPerson = new Guid();
        fake.Appointments = [];
        fake.Bonus = 0;
        fake.DateOfBirth = DateTime.Now;
        fake.OvertimePerMonth = 0;
        fake.Patients = [];
        fake.Qualifications = [];
        fake.Address = "korobishcheee";
        fake.IdsPatients = [];
        fake.Name = "cccc";
        fake.Supervisor = null;
        fake.Salary = 0;
        fake.Subordinates = [];
        fake.DateHired = DateTime.Now;
        fake.DateFired = null;
        fake.IdSupervisor = null;
        
        var ex = Assert.Throws<ValidationException>((() => 
            _personFactory.CreateNewNurse(fake, "ddddd", "ddd", DateTime.Now, "korobochka")));
        Assert.That(ex.Errors.Count(),Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x=>x.ErrorMessage=="No such employee found."),Is.EqualTo(1));
    }
    
    [Test]
    public void TherapistNullQualificationsTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewTherapist(null, "1", "2",DateTime.MinValue, "korobka",null!));
        Assert.That(ex.Errors.Count() , Is.EqualTo(5));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Therapist must have at least one qualification.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeNullSupervisorTest()
    {
        Assert.DoesNotThrow(() =>
            _personFactory.CreateNewTherapist(null, "12", "22",DateTime.Now, "korobochka",["medal"]));
    }
    
    [Test]
    public void EmployeeNullNameTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, null!, "helpovich", DateTime.Now, "korobochka_lesnaya"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name cannot be null.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeNullSurnameTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, null!, null!, DateTime.Now, "korobochka_lesnaya"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(2));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name cannot be null.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname cannot be null.") , Is.EqualTo(1));
    }
    
    [Test]
    public void EmployeeNullAddressTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, null!, null!, DateTime.Now, null!));
        Assert.That(ex.Errors.Count() , Is.EqualTo(3));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name cannot be null.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname cannot be null.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address cannot be null.") , Is.EqualTo(1));
    }
    
    [Test]
    public void TherapistConnectionsTest()
    {
        var therapist =
            _personFactory.CreateNewTherapist(null, "horoshii", "doctor", DateTime.Now, "korobochka", ["d"]);
        var patient1 = _personFactory.CreateNewPatient("pervyi", "pervovich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        var patient2 = _personFactory.CreateNewPatient("vtoroi", "vtoroevich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        var patient3 = _personFactory.CreateNewPatient("tretii", "tretievich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        therapist.AddPatient(patient1);
        therapist.AddPatient(patient2);
        therapist.AddPatient(patient3);
        _storageManager.Serialize();
        _personStorage.Delete(therapist);
        _personStorage.Delete(patient1);
        _personStorage.Delete(patient2);
        _personStorage.Delete(patient3);
        _storageManager.Deserialize();
        
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==therapist.IdPerson).First()as Therapist)?.Patients.Count,Is.EqualTo(3));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==patient1.IdPerson).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==patient2.IdPerson).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==patient3.IdPerson).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
    }
    
    [Test]
    public void NurseConnectionsTest()
    {
        var nurse =
            _personFactory.CreateNewNurse(null, "horoshii", "doctor", DateTime.Now, "korobochka");
        var room1 = _roomFactory.CreateNewRoom(3);
        var room2 = _roomFactory.CreateNewRoom(2);
        var room3 = _roomFactory.CreateNewRoom(1);
        
        nurse.AddRoom(room1);
        nurse.AddRoom(room2);
        nurse.AddRoom(room3);
        _storageManager.Serialize();
        _personStorage.Delete(nurse);
        _roomStorage.Delete(room2);
        _roomStorage.Delete(room1);
        _roomStorage.Delete(room3);
        _storageManager.Deserialize();
        
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==nurse.IdPerson).First()as Nurse)?.Rooms.Count,Is.EqualTo(3));
        Assert.That((_roomStorage.FindBy(x=>x.IdRoom==room1.IdRoom).First())?.Nurses.Count,Is.EqualTo(1));
        Assert.That((_roomStorage.FindBy(x=>x.IdRoom==room2.IdRoom).First())?.Nurses.Count,Is.EqualTo(1));
        Assert.That((_roomStorage.FindBy(x=>x.IdRoom==room3.IdRoom).First())?.Nurses.Count,Is.EqualTo(1));
    }
    
    [Test]
    public void NurseConnectionsNonDeletedTest()
    {
        var nurse =
            _personFactory.CreateNewNurse(null, "horoshii", "doctor", DateTime.Now, "korobochka");
        var room1 = _roomFactory.CreateNewRoom(3);
        var room2 = _roomFactory.CreateNewRoom(2);
        var room3 = _roomFactory.CreateNewRoom(1);
        
        nurse.AddRoom(room1);
        nurse.AddRoom(room2);
        nurse.AddRoom(room3);
        _storageManager.Serialize();
        _storageManager.Deserialize();
        Assert.That(_personStorage.Count,Is.EqualTo(1));
        Assert.That(_roomStorage.Count,Is.EqualTo(3));

        Assert.That((_personStorage.FindBy(x=>x.IdPerson==nurse.IdPerson).First()as Nurse)?.Rooms.Count,Is.EqualTo(3));
        Assert.That((_roomStorage.FindBy(x=>x.IdRoom==room1.IdRoom).First())?.Nurses.Count,Is.EqualTo(1));
        Assert.That((_roomStorage.FindBy(x=>x.IdRoom==room2.IdRoom).First())?.Nurses.Count,Is.EqualTo(1));
        Assert.That((_roomStorage.FindBy(x=>x.IdRoom==room3.IdRoom).First())?.Nurses.Count,Is.EqualTo(1));
    }
    
    [Test]
    public void TherapistConnectionsNonDeletedTest()
    {
        var therapist =
            _personFactory.CreateNewTherapist(null, "horoshii", "doctor", DateTime.Now, "korobochka", ["d"]);
        var patient1 = _personFactory.CreateNewPatient("pervyi", "pervovich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        var patient2 = _personFactory.CreateNewPatient("vtoroi", "vtoroevich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        var patient3 = _personFactory.CreateNewPatient("tretii", "tretievich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        
        therapist.AddPatient(patient1);
        therapist.AddPatient(patient2);
        therapist.AddPatient(patient3);
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        Assert.That(_personStorage.Count,Is.EqualTo(4));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==therapist.IdPerson).First()as Therapist)?.Patients.Count,Is.EqualTo(3));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==patient1.IdPerson).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==patient2.IdPerson).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
        Assert.That((_personStorage.FindBy(x=>x.IdPerson==patient3.IdPerson).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
    }
    
     [Test]
    public void StorageInnerSerializeTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, LevelOfDanger.High, true);
       var str = _diagnosisStorage.Serialize();
       
       
        _diagnosisStorage.Deserialize(str);
        var res = _diagnosisStorage.FindBy(d => true);

    }

    [Test]
    public void DiagnosisSerializeTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereAnxiety
        (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null,
            LevelOfDanger.High, true);
        
        var foundPatient = (_personStorage.FindBy(x => x.IdPerson == patient.IdPerson).First() as Patient);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(_diagnosisStorage.FindBy(x => x.IdPatient == patient.IdPerson).ToList().Count, Is.EqualTo(2));
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
         foundPatient = (_personStorage.FindBy(x => x.IdPerson == patient.IdPerson).First() as Patient);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(_diagnosisStorage.FindBy(x => x.IdPatient == patient.IdPerson).ToList().Count, Is.EqualTo(2));
    }
    
    [Test]
    public void BigManagerSerializeTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereAnxiety
        (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null,
            LevelOfDanger.High, true);
        var room = _roomFactory.CreateNewRoom(3);
        var thersp = _personFactory.CreateNewTherapist(null, "Max", "Verstappen", DateTime.Now, "Melbourne, Australia",
            new[] { "finished high school" });
        thersp.AddPatient(patient);
        patient.Therapists.Add(thersp);
        var nurse = _personFactory.CreateNewNurse(null, "Pomogite", "Pomogovich", DateTime.Now, "Korobusikaaaa");
        var nurse2 = _personFactory.CreateNewNurse(thersp, "Lewis", "Hamilton", DateTime.Now, "Monte-Carlo, Monaco");
        room.Nurses.Add(nurse);
        nurse.AddRoom(room);
        var rp = _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, DateTime.Now.AddDays(2));
        var appoint =
            _appointmentFactory.CreateNewAppointment(thersp, patient, DateTime.Now,
                "Patient needed some strong medication immediately.");
        var prescr = _prescriptionFactory.CreateNewPrescription(appoint, "Anti-stress pills", 30, 5.5m, 
            "very important prescription for your live");
        
        Assert.That(_personStorage.Count , Is.EqualTo(4));
        Assert.That(_diagnosisStorage.Count , Is.EqualTo(2));
        Assert.That(_roomStorage.Count , Is.EqualTo(1));
        
       
        var foundPatient = (_personStorage.FindBy(x => x.IdPerson == patient.IdPerson).First() as Patient);
        var foundTherapist = (_personStorage.FindBy(x => x.IdPerson == thersp.IdPerson).First() as Therapist);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(foundPatient?.Therapists.Count , Is.EqualTo(1));
        Assert.That(foundTherapist?.Patients.Count , Is.EqualTo(1));
        var foundNurse2 = (_personStorage.FindBy(x => x.IdPerson == nurse2.IdPerson).First() as Nurse);
        Assert.That(foundTherapist?.Subordinates.Count , Is.EqualTo(1));
        Assert.That(foundNurse2?.Supervisor , !Is.Null);
        var foundNurse = (_personStorage.FindBy(x => x.IdPerson == nurse.IdPerson).First() as Nurse);
        Assert.That(foundNurse?.Rooms.Count , Is.EqualTo(1));
        var foundRoom = _roomStorage.FindBy(x => x.IdRoom == room.IdRoom).First();
        Assert.That(foundRoom?.Nurses.Count , Is.EqualTo(1));
        var foundAppoint = _appointmentStorage.FindBy(x => x.IdAppointment == appoint.IdAppointment).First();
        Assert.That(foundAppoint?.Therapist.IdPerson , Is.EqualTo(foundTherapist?.IdPerson));
        Assert.That(foundAppoint?.Patient?.IdPerson , Is.EqualTo(foundPatient?.IdPerson));
        var foundPrescr = _prescriptionStorage.FindBy(x => x.IdPrescription == prescr.IdPrescription).First();
        Assert.That(foundPrescr?.IdAppointment , Is.EqualTo(foundAppoint?.IdAppointment));
     
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        
         foundPatient = (_personStorage.FindBy(x => x.IdPerson == patient.IdPerson).First() as Patient);
         foundTherapist = (_personStorage.FindBy(x => x.IdPerson == thersp.IdPerson).First() as Therapist);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(foundPatient?.Therapists.Count , Is.EqualTo(1));
        Assert.That(foundTherapist?.Patients.Count , Is.EqualTo(1));
         foundNurse2 = (_personStorage.FindBy(x => x.IdPerson == nurse2.IdPerson).First() as Nurse);
        Assert.That(foundTherapist?.Subordinates.Count , Is.EqualTo(1));
        Assert.That(foundNurse2?.Supervisor , !Is.Null);
         foundNurse = (_personStorage.FindBy(x => x.IdPerson == nurse.IdPerson).First() as Nurse);
        Assert.That(foundNurse?.Rooms.Count , Is.EqualTo(1));
         foundRoom = _roomStorage.FindBy(x => x.IdRoom == room.IdRoom).First();
        Assert.That(foundRoom?.Nurses.Count , Is.EqualTo(1));
         foundAppoint = _appointmentStorage.FindBy(x => x.IdAppointment == appoint.IdAppointment).First();
        Assert.That(foundAppoint?.Therapist.IdPerson , Is.EqualTo(foundTherapist?.IdPerson));
        Assert.That(foundAppoint?.Patient?.IdPerson , Is.EqualTo(foundPatient?.IdPerson));
         foundPrescr = _prescriptionStorage.FindBy(x => x.IdPrescription == prescr.IdPrescription).First();
        Assert.That(foundPrescr?.IdAppointment , Is.EqualTo(foundAppoint?.IdAppointment));
        
    }
}