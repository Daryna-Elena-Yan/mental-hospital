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
    private FileService _fileService;
    private LightDiagnosisValidator _lightDiagnosisValidator;
    private SevereDiagnosisValidator _severeDiagnosisValidator;
    private Storage<Person> _personStorage;

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
        _lightDiagnosisValidator = provider.GetRequiredService<LightDiagnosisValidator>();
        _severeDiagnosisValidator = provider.GetRequiredService<SevereDiagnosisValidator>();

        _fileService = provider.GetRequiredService<FileService>();
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
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Today,"korobka", []);
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(therapist1.Name, Is.EqualTo("frst"));
            Assert.That(therapist1.Bonus, Is.EqualTo(0));
            Assert.That(therapist1.Address, Is.EqualTo("korobka"));
            Assert.That(therapist1.Surname, Is.EqualTo("frstovich"));
            Assert.That(therapist1.DateHired, Is.EqualTo(DateTime.Today));
            Assert.That(therapist1.Supervisor, Is.Null);
            Assert.That(therapist1.DateOfBirth, Is.EqualTo(DateTime.Today));
            Assert.That(therapist1.Qualifications, Is.EqualTo(new List<string>()));
            Assert.That(therapist1.OvertimePerMonth, Is.EqualTo(0));
        });
    }
    [Test]
    public void TherapistStaticDerivedTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
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
        var nurse =  _personFactory.CreateNewNurse(null, "frst", "frstovich", DateTime.Now,"korobka");
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
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        _personStorage.Delete(therapist1);
        Assert.That(_personStorage.Count, Is.EqualTo(0));
}
    [Test]
    public void TherapistWithSupervisorCreationTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
        var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", []);
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
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
        var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", []);
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
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
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
            var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
            var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", []);
            var appointment = _appointmentFactory.CreateNewAppointment(therapist2, null, DateTime.Now, "help");
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
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        Assert.Multiple(() =>
        {
            Assert.That(_fileService.GetStorage<Person>().Count, Is.EqualTo(2));
            Assert.That(_fileService.GetStorage<Appointment>().Count, Is.EqualTo(1));

            Assert.That(appointment.Patient, !Is.Null);
            Assert.That(appointment.Therapist, !Is.Null);
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(patient.Appointments.Count, Is.EqualTo(1));
        });

        _appointmentStorage.Delete(appointment);
        Assert.Multiple(() =>
        {
            Assert.That(_fileService.GetStorage<Person>().Count, Is.EqualTo(2));
            Assert.That(_fileService.GetStorage<Appointment>().Count, Is.EqualTo(0));
           
            Assert.That(therapist.Appointments.Count, Is.EqualTo(0));
            Assert.That(patient.Appointments.Count, Is.EqualTo(0));
        });
    }
    [Test]
        public void TherapistWithSupervisorAndAppointmentDeletionTest()
        {
            var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
            var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", []);
            var appointment = _appointmentFactory.CreateNewAppointment(therapist2, null, DateTime.Now, "help");
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
            DateTime.Now,"Baker Street, 221B", []);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
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
            Assert.That(_personStorage.Count, Is.EqualTo(0));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(therapist.Appointments.Count, Is.EqualTo(1));
            Assert.That(appointment.Patient, Is.Null);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
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
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, "do sth");
        var prescriptionAnother = _prescriptionFactory.CreateNewPrescription(appointment, "Be not healthy", 12, 0.2m, "not do sth");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
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
                var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobka", []);
                var therapist2 =  _personFactory.CreateNewTherapist(therapist1, "scnd", "scndovich", DateTime.Now,"korobochka", []);
                var appointment = _appointmentFactory.CreateNewAppointment(therapist2, null, DateTime.Now, "help"); 
                var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "stay strong", 30, 0.02m, "hallucinations");
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
            DateTime.Now,"Baker Street, 221B", []);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, "sth");
        
        var prescription = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, "do sth");
        Assert.Multiple(() =>
        {
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
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
            Assert.That(_personStorage.Count, Is.EqualTo(1));
            Assert.That(_personStorage.Count, Is.EqualTo(1));
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
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should not be empty.") , Is.EqualTo(1));
            
    }
    
    [Test]
    public void PatientEmptySurnameAttributeValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should not be empty.") , Is.EqualTo(1));
            
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
        var format = "dd/MM/yyyy";
        var culture = CultureInfo.InvariantCulture;

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
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should not be empty.") , Is.EqualTo(1));
            
    }

    [Test]
    public void SerializeTest()
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

    
    

    // [Test]
    // public void SerializationTest()
    // {
    //     var room = _roomFactory.CreateNewRoom(3);
    //     var g = room.IdRoom;
    //    
    //     var nurse = _personFactory.CreateNewNurse(null, "Pomogite", "Pomogovich", DateTime.Now, "Korobusik");
    //     var bd = nurse.DateOfBirth;
    //     var d = nurse.IdPerson;
    //     nurse.Rooms.Add(room);
    //     room.Nurses.Add(nurse);
    //     nurse.IdsRooms.Add(room.IdRoom);
    //     Assert.That(_roomStorage.Count,Is.EqualTo(1));
    //     Assert.That(_personStorage.Count,Is.EqualTo(1));
    //     _fileService.Serialize();
    //     _roomStorage.Delete(room);
    //     _personStorage.Delete(nurse);
    //     Assert.That(_roomStorage.Count,Is.EqualTo(0));
    //     Assert.That(_personStorage.Count,Is.EqualTo(0));
    //     _fileService.Deserialize();
    //     Assert.That(g,Is.EqualTo(_roomStorage.FindBy(room1 =>room1.IdRoom==g).First().IdRoom));
    //     Assert.That(_roomStorage.Count,Is.EqualTo(1));
    //     Assert.That(_personStorage.Count,Is.EqualTo(1));
    //     Assert.That(FileService.GetString(),
    //         Is.EqualTo("{\"Patients\":[],\"Nurses\":[{\"IdsRooms\":[\""+g+"\"],\"Bonus\":0," +
    //                    "\"OvertimePerMonth\":0,\"Salary\":6000,\"DateHired\":\"2024-11-07T00:00:00+01:00\"," +
    //                    "\"DateFired\":null,\"IdSupervisor\":null,\"IdPerson\":\""+d+"\",\"Name\":\"Pomogite" +
    //                    "\",\"Surname\":\"Pomogovich\",\"DateOfBirth\":\""+bd.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz")+"\"," +
    //                    "\"Address\":\"Korobusik\"}],\"Therapists\":[],\"Diagnoses\":[],\"Appointments\":[]," +
    //                    "\"Equipments\":[],\"Prescriptions\":[],\"Rooms\":[{\"IdRoom\":\""+g+"\",\"Quantity\"" +
    //                    ":3}],\"RoomPatients\":[]}"));
    //     /*Assert.That(FileService.GetString(),
    //         Is.EqualTo("[{\"IdRoom\":\""+g+
    //                    "\",\"Quantity\":3,\"Nurses\":[],\"Equipments\":[],\"RoomPatients\":[]}]"));*/
    // }
}