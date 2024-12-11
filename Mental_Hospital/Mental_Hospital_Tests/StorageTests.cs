using System.Globalization;
using System.Security.AccessControl;
using FluentValidation;
using Mental_Hospital;
using Mental_Hospital.Collections;
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
    private ServiceProvider _provider;

    [SetUp]
    public void Setup()
    {
        // Initialize DI container
        var services = new ServiceCollection();
        services.MentalHospitalSetup();
        _provider = services.BuildServiceProvider();

        // Get service instances for tests
        _personFactory = _provider.GetRequiredService<PersonFactory>();
        _diagnosisFactory = _provider.GetRequiredService<DiagnosisFactory>();
        _appointmentFactory = _provider.GetRequiredService<AppointmentFactory>();
        _equipmentFactory = _provider.GetRequiredService<EquipmentFactory>();
        _roomFactory = _provider.GetRequiredService<RoomFactory>();
        _roomPatientFactory = _provider.GetRequiredService<RoomPatientFactory>();
        _prescriptionFactory = _provider.GetRequiredService<PrescriptionFactory>();
        
        _personStorage = _provider.GetRequiredService<Storage<Person>>();
        _diagnosisStorage = _provider.GetRequiredService<Storage<Diagnosis>>();
        _appointmentStorage = _provider.GetRequiredService<Storage<Appointment>>();
        _equipmentStorage = _provider.GetRequiredService<Storage<Equipment>>();
        _roomStorage = _provider.GetRequiredService<Storage<Room>>();
        _roomPatientStorage = _provider.GetRequiredService<Storage<RoomPatient>>();
        _prescriptionStorage = _provider.GetRequiredService<Storage<Prescription>>();

        _storageManager = _provider.GetRequiredService<StorageManager>();
    }

    [TearDown]
    public void TearDown()
    {
        _provider.Dispose();
    }

    [Test, Category("Patient"), Category("Register")]
    public void PatientRegisterTest()
    {
        _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
    }
    
    [Test, Category("Therapist"), Category("Register")]
    public void TherapistRegisterTest()
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
    
    [Test, Category("Therapist"), Category("Register")]
    public void TherapistStaticDerivedRegisterTest()
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

    [Test, Category("Nurse"), Category("Register")]
    public void NurseRegisterTest()
    {
        var nurse = _personFactory.CreateNewNurse(null, "aaa", "AAA", DateTime.Today, "korobochka");
        Assert.That(_personStorage.Count,Is.EqualTo(1));
        var room = _roomFactory.CreateNewRoom(3);
    }

    [Test, Category("Nurse"), Category("Delete")]
    public void NurseDeleteTest()
    {
        var nurse = _personFactory.CreateNewNurse(null, "aaa", "AAA", DateTime.Today, "korobochka");
        Assert.That(_personStorage.Count,Is.EqualTo(1));
        _personStorage.Delete(nurse);
        Assert.That(_personStorage.Count,Is.EqualTo(0));
    }

    
    [Test, Category("Nurse"), Category("Register")]
    public void NurseStaticDerivedRegisterTest()
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
    
    [Test, Category("Therapist"), Category("Delete")]
    public void TherapistDeleteTest()
    {
        var therapist1 =  _personFactory.CreateNewTherapist(null, "frst", "frstovich", DateTime.Now,"korobkaaaaa", ["s"]);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        _personStorage.Delete(therapist1);
        Assert.That(_personStorage.Count, Is.EqualTo(0));
    }
    
    [Test, Category("Therapist"), Category("Register")]
    public void TherapistWithSupervisorRegisterTest()
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
    
    [Test, Category("Therapist"), Category("Delete")]
    public void TherapistWithSupervisorDeleteTest()
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
    
    [Test, Category("Diagnosis"), Category("Register")]
    public void DiagnosisRegisterTest()
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
    
    [Test, Category("Diagnosis"), Category("Delete")]
    public void DiagnosisDeleteTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        _diagnosisStorage.Delete(diagnosis);
        Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
        Assert.That(patient.Diagnoses.Where(x => x == diagnosis).Count, Is.EqualTo(0));
    }
    
    [Test, Category("Patient"), Category("Delete")]
    public void PatientWithDiagnosesDeleteTest()
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
    
    [Test, Category("Room"), Category("Register")]
    public void RoomRegisterTest()
    {
        _roomFactory.CreateNewRoom(3);
        Assert.That(_roomStorage.Count, Is.EqualTo(1));
    }
    
    [Test, Category("Nurse"), Category("Register")]
    public void NurseWithRoomsRegisterTest()
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
    
    [Test, Category("Room"), Category("Delete")]
    public void RoomDeleteTest()
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
    
    [Test, Category("RoomPatient"), Category("Register")]
    public void RoomPatientRegisterTest()
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
    
    [Test, Category("RoomPatient"), Category("Delete")]
    public void RoomPatientDeleteTest()
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
    
    [Test, Category("Patient"), Category("Delete")]
    public void PatientWithRoomPatientDeleteTest()
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
    
    [Test, Category("Appointment"), Category("Register")]
    public void AppointmentRegisterTest()
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
    
    [Test, Category("Therapist"), Category("Register")]
    public void TherapistWithSupervisorAndAppointmentRegisterTest()
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
    
    [Test, Category("Appointment"), Category("Delete")]
    public void AppointmentDeleteTest()
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
    
    [Test, Category("Therapist"), Category("Delete")]
    public void TherapistWithSupervisorAndAppointmentDeleteTest()
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
    
    [Test, Category("Patient"), Category("Delete")]
    public void PatientWithAppointmentDeleteTest()
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
    
    [Test, Category("Prescription"), Category("Register")]
    public void PrescriptionRegisterTest()
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

        Assert.IsTrue(appointment.Prescriptions.ContainsKey(prescription.Id));
        Assert.IsTrue(appointment.Prescriptions[prescription.Id].Equals(prescription));
    }
    
    [Test, Category("Prescription"), Category("Delete")]
    public void PrescriptionDeleteTest()
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

        Assert.IsTrue(appointment.Prescriptions.ContainsKey(prescription.Id));
        Assert.IsTrue(appointment.Prescriptions[prescription.Id].Equals(prescription));
        
        _prescriptionStorage.Delete(prescription);
        Assert.Multiple(() =>
        {
            Assert.That(_prescriptionStorage.Count, Is.EqualTo(1));
            Assert.That(_appointmentStorage.Count, Is.EqualTo(1));
            Assert.That(appointment.Prescriptions.Count, Is.EqualTo(1));
        });
        Assert.IsFalse(appointment.Prescriptions.ContainsKey(prescription.Id));
    }
    
    [Test, Category("Therapist"), Category("Delete")]
    public void TherapistWithSupervisorAndAppointmentAndPrescriptionDeleteTest()
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
    
    [Test, Category("Appointment"), Category("Delete")]
    public void AppointmentWithPrescriptionDeleteTest()
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
    
    
    [Test, Category("Equipment"), Category("Register")]
    public void EquipmentRegisterTest()
    {
        _equipmentFactory.CreateNewEquipment("IV stand", DateTime.Today);
        Assert.That(_equipmentStorage.Count, Is.EqualTo(1));
    }
    
    [Test, Category("Equipment"), Category("Delete")]
    public void EquipmentDeleteTest()
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
    
    [Test, Category("Room"), Category("Delete")]
    public void RoomWithEquipmentDeleteTest()
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
    
    [Test, Category("Patient"), Category("Validation"), Category("PatientValidator")]
    public void PatientEmptyNameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
            
    }
    
    [Test, Category("Patient"), Category("Validation"), Category("PatientValidator")]
    public void PatientEmptySurnameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
            
    }
    
    [Test, Category("Patient"), Category("Validation"), Category("PatientValidator")]
    public void PatientDeathEarlierThatBirthValidationTest()
    {
        var death =  DateTime.ParseExact("25/12/1999",format,culture);
        var birth = DateTime.ParseExact("25/12/2002",format,culture);

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", birth, "Melbourne, Australia",
            "cases of selfharm in the past", death));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Date of healing cannot be earlier that date of Diagnosing.") , Is.EqualTo(1));
            
    }
    
    [Test, Category("Patient"), Category("Validation"), Category("PatientValidator")]
    public void PatientEmptyAddressValidationTest()
    {
        var birth = DateTime.ParseExact("25/12/2002",format,culture);

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", birth, "",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
            
    }
    
    [Test, Category("Patient"), Category("Validation"), Category("PatientValidator")]
    public void PatientLongAddressValidationTest()
    {
        var birth = DateTime.ParseExact("25/12/2002",format,culture);

        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", birth, "Austrlia is a very nice country but there are spiders and insect but i think that kangoroos are cute",
            "cases of selfharm in the past", null));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
            
    }

    [Test, Category("Patient"), Category("Validation"), Category("PatientValidator")]
    public void PatientNullDateBirthValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.MinValue, 
            "Melbourne, Australia",
            "cases of selfharm in the past", null));

        Assert.That(ex.Errors.Count(), Is.EqualTo(1));
        Assert.That(
            ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth"),
            Is.EqualTo(1));

    }

    [Test, Category("Diagnosis"), Category("Validation"), Category("DiagnosisValidator")]
    public void DiagnosisEmptyNameValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        var ex = Assert.Throws<ValidationException>(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "",
            "cases of anexity related to the past",
            new[] { "swap positions" }, DateTime.Now, DateTime.Now.AddDays(1), true));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify name of the Diagnosis.") , Is.EqualTo(1));
            
    }
    
    [Test, Category("Diagnosis"), Category("Validation"), Category("DiagnosisValidator")]
    public void DiagnosisShortDescriptionValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        var ex = Assert.Throws<ValidationException>(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
            "cases",
            new[] { "swap positions" }, DateTime.Now, DateTime.Now.AddDays(1), true));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Description should be from 20 to 500 symbols long.") , Is.EqualTo(1));
    }
    
    [Test, Category("Diagnosis"), Category("Validation"), Category("DiagnosisValidator")]
    public void DiagnosisLongDescriptionValidationTest()
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
    
    [Test, Category("Diagnosis"), Category("Validation"), Category("DiagnosisValidator")]
    public void DiagnosisHealingDateBeforeDiagnosisDateValidationTest()
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
    
    
    [Test, Category("Diagnosis"), Category("Validation"), Category("DiagnosisValidator")]
    public void DiagnosisHealingIsNullValidationTest()
    {
        var patient = _personFactory.CreateNewPatient("Oscar", "Piastri", DateTime.Now, "Melbourne, Australia",
            "cases of selfharm in the past", null);

        Assert.DoesNotThrow(() => _diagnosisFactory.CreateNewLightAnxiety(patient, "PTS",
            "cases of anexity related to the past",
            new[] { "swap positions" }, DateTime.Now.AddDays(1), null, true));
    }
    
    [Test, Category("Diagnosis"), Category("Validation"), Category("DiagnosisValidator")]
    public void DiagnosisPatientIncorrectValidationTest()
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
    
    [Test, Category("Appointment"), Category("Validation"), Category("AppointmentValidator")]
    public void AppointmentNullDateOfAppointmentValidationTest()
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
    
    [Test, Category("Appointment"), Category("Validation"), Category("AppointmentValidator")]
    public void AppointmentShortDescriptionValidationTest()
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
    
    [Test, Category("Appointment"), Category("Validation"), Category("AppointmentValidator")]
    public void AppointmentLongDescriptionValidationTest()
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
    
    [Test, Category("Appointment"), Category("Validation"), Category("AppointmentValidator")]
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
    
    [Test, Category("Appointment"), Category("Validation"), Category("AppointmentValidator")]
    public void AppointmentNullPatientValidationTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["d"]);

        Assert.DoesNotThrow(() => _appointmentFactory.CreateNewAppointment(therapist, null, DateTime.Now,
                "very important appointment for your live"));
    }
    
    [Test, Category("Appointment"), Category("Validation"), Category("AppointmentValidator")]
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
    
    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionNullNameValidationTest()
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
    
    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionEmptyNameValidationTest()
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
    
    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionNegativeQuantityValidationTest()
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
    
    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionNegativeDosageValidationTest()
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

    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionShortDescriptionValidationTest()
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

    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionLongDescriptionValidationTest()
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
    
    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
    public void PrescriptionNullAppointmentValidationTest()
    {
        Assert.DoesNotThrow(() =>  _prescriptionFactory.CreateNewPrescription(null, 
            "Be healthy", 10, 0.02m, "very important prescription for your live"));;
    }
    
    [Test, Category("Prescription"), Category("Validation"), Category("PrescriptionValidator")]
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
    
    [Test, Category("Equipment"), Category("Validation"), Category("EquipmentValidator")]
    public void EquipmentNullNameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => 
            _equipmentFactory.CreateNewEquipment(null!, DateTime.Today));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Please specify name.") , Is.EqualTo(1));
    }
    
    [Test, Category("Equipment"), Category("Validation"), Category("EquipmentValidator")]
    public void EquipmentEmptyNameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => 
            _equipmentFactory.CreateNewEquipment("", DateTime.Today));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 1 characters long.") , Is.EqualTo(1));
    }
    
    [Test, Category("Equipment"), Category("Validation"), Category("EquipmentValidator")]
    public void EquipmentNullExpirationDateValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => 
            _equipmentFactory.CreateNewEquipment("Some stuff", DateTime.MinValue));

        Assert.That(ex.Errors.Count(), Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of expiration."),
            Is.EqualTo(1));
    }
    
    [Test, Category("Room"), Category("Validation"), Category("RoomValidator")]
    public void RoomNegativeCapacityValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => _roomFactory.CreateNewRoom(-3));
        
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Capacity must be greater than or equal to 0.") , Is.EqualTo(1));
    }
    
    [Test, Category("RoomPatient"), Category("Validation"), Category("RoomPatientValidator")]
    public void RoomPatientNullDatePlacedValidationTest()
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
    
    [Test, Category("RoomPatient"), Category("Validation"), Category("RoomPatientValidator")]
    public void RoomPatientDatePlacedAfterDateDischargedValidationTest()
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
    
    [Test, Category("RoomPatient"), Category("Validation"), Category("RoomPatientValidator")]
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
    
    [Test, Category("RoomPatient"), Category("Validation"), Category("RoomPatientValidator")]
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
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeShortNameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() => 
            _personFactory.CreateNewNurse(null, "1", "helpovich", DateTime.Now, "korobochka_lesnaya"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
    }
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeShortSurnameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, "1", "2", DateTime.Now, "korobochka"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(2));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
    }
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeNullDateOfBirthValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, "1", "2",DateTime.MinValue, "korobochka"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(3));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth") , Is.EqualTo(1));
    }
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeShortAddressValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, "1", "2",DateTime.MinValue, "korobka"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(4));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname should be at least 2 characters long.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Specify date of birth") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address must be of length from 10 to 70 symbols.") , Is.EqualTo(1));
    }
    
    [Test, Category("Therapist"), Category("Validation"), Category("TherapistValidator")]
    public void TherapistEmptyQualificationsValidationTest()
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

    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeSupervisorDoesNotExistValidationTest()
    {
        var fake = new Therapist();
        fake.Id = new Guid();
        fake.Appointments = [];
        fake.Bonus = 0;
        fake.DateOfBirth = DateTime.Now;
        fake.OvertimePerMonth = 0;
        fake.Patients = new AssociationCollection<Patient>(_provider);
        fake.Qualifications = [];
        fake.Address = "korobishcheee";
        fake.Name = "cccc";
        fake.Supervisor = null;
        fake.Salary = 0;
        fake.DateHired = DateTime.Now;
        fake.DateFired = null;
        fake.IdSupervisor = null;
        
        var ex = Assert.Throws<ValidationException>((() => 
            _personFactory.CreateNewNurse(fake, "ddddd", "ddd", DateTime.Now, "korobochka")));
        Assert.That(ex.Errors.Count(),Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x=>x.ErrorMessage=="No such employee found."),Is.EqualTo(1));
    }
    
    [Test, Category("Therapist"), Category("Validation"), Category("TherapistValidator")]
    public void TherapistNullQualificationsValidationTest()
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
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeNullSupervisorValidationTest()
    {
        Assert.DoesNotThrow(() =>
            _personFactory.CreateNewTherapist(null, "12", "22",DateTime.Now, "korobochka",["medal"]));
    }
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeNullNameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, null!, "helpovich", DateTime.Now, "korobochka_lesnaya"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name cannot be null.") , Is.EqualTo(1));
    }
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeNullSurnameValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, null!, null!, DateTime.Now, "korobochka_lesnaya"));
        Assert.That(ex.Errors.Count() , Is.EqualTo(2));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name cannot be null.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname cannot be null.") , Is.EqualTo(1));
    }
    
    [Test, Category("Employee"), Category("Validation"), Category("EmployeeValidator")]
    public void EmployeeNullAddressValidationTest()
    {
        var ex = Assert.Throws<ValidationException>(() =>
            _personFactory.CreateNewNurse(null, null!, null!, DateTime.Now, null!));
        Assert.That(ex.Errors.Count() , Is.EqualTo(3));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Name cannot be null.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Surname cannot be null.") , Is.EqualTo(1));
        Assert.That(ex.Errors.Count(x => x.ErrorMessage == "Address cannot be null.") , Is.EqualTo(1));
    }
    
    [Test, Category("Nurse"), Category("Serialize")]
    public void NurseSerializeTest()
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

        Assert.That((_personStorage.FindBy(x=>x.Id==nurse.Id).First()as Nurse)?.Rooms.Count,Is.EqualTo(3));
        Assert.That((_roomStorage.FindBy(x=>x.Id==room1.Id).First())?.Nurses.Count,Is.EqualTo(1));
        Assert.That((_roomStorage.FindBy(x=>x.Id==room2.Id).First())?.Nurses.Count,Is.EqualTo(1));
        Assert.That((_roomStorage.FindBy(x=>x.Id==room3.Id).First())?.Nurses.Count,Is.EqualTo(1));
    }
    
    [Test, Category("Therapist"), Category("Serialize")]
    public void TherapistSerializeTest()
    {
        var therapist =
            _personFactory.CreateNewTherapist(null, "horoshii", "doctor", DateTime.Now, "korobochka", ["d"]);
        var patient1 = _personFactory.CreateNewPatient("pervyi", "pervovich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        var patient2 = _personFactory.CreateNewPatient("vtoroi", "vtoroevich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        var patient3 = _personFactory.CreateNewPatient("tretii", "tretievich", DateTime.Now,
            "korobochka", "vse ochen ploho, tut absolutno bez slov, zhizn bol", null);
        
        therapist.Patients.Add(patient1);
        therapist.Patients.Add(patient2);
        therapist.Patients.Add(patient3);
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        Assert.That(_personStorage.Count,Is.EqualTo(4));
        Assert.That((_personStorage.FindBy(x=>x.Id==therapist.Id).First()as Therapist)?.Patients.Count,Is.EqualTo(3));
        Assert.That((_personStorage.FindBy(x=>x.Id==patient1.Id).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
        Assert.That((_personStorage.FindBy(x=>x.Id==patient2.Id).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
        Assert.That((_personStorage.FindBy(x=>x.Id==patient3.Id).First()as Patient)?.Therapists.Count,Is.EqualTo(1));
    }
    
    [Test, Category("Appointment"), Category("Serialize")]
    public void AppointmentSerializeTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["diploma"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var appointment1 = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        var appointment2 = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        
        var foundTherapist = (_personStorage.FindBy(x => x.Id == therapist.Id).First() as Therapist);
        Assert.That(foundTherapist?.Appointments.Count, Is.EqualTo(2));
        var foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        Assert.That(foundPatient?.Appointments.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage
            .FindBy(x => x.IdTherapist == therapist.Id && x.IdPatient == patient.Id).ToList().Count, Is.EqualTo(2));
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        foundTherapist = (_personStorage.FindBy(x => x.Id == therapist.Id).First() as Therapist);
        Assert.That(foundTherapist?.Appointments.Count, Is.EqualTo(2));
        foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        Assert.That(foundPatient?.Appointments.Count, Is.EqualTo(2));
        Assert.That(_appointmentStorage
            .FindBy(x => x.IdTherapist == therapist.Id && x.IdPatient == patient.Id).ToList().Count, Is.EqualTo(2));
    }
    
    [Test, Category("Prescription"), Category("Serialize")]
    public void PrescriptionSerializeTest()
    {
        var therapist =  _personFactory.CreateNewTherapist(null, "Charles", "Leclerc", 
            DateTime.Now,"Baker Street, 221B", ["diploma"]);
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var appointment = _appointmentFactory.CreateNewAppointment(therapist, patient, DateTime.Now, 
            "very important appointment for your live");
        var prescription1 = _prescriptionFactory.CreateNewPrescription(appointment, "Be very healthy", 20, 0.15m, 
            "very important prescription for your live");
        var prescription2 = _prescriptionFactory.CreateNewPrescription(appointment, "Be healthy", 10, 0.02m, 
            "very important prescription for your live");
        
        var foundAppointment = (_appointmentStorage.FindBy(x => x.Id == appointment.Id).First());
        Assert.That(foundAppointment?.Prescriptions.Count, Is.EqualTo(2));
        Assert.IsTrue(foundAppointment?.Prescriptions.ContainsKey(prescription1.Id));
        Assert.IsTrue(foundAppointment?.Prescriptions.ContainsKey(prescription2.Id));
        Assert.That(_prescriptionStorage
            .FindBy(x => x.IdAppointment == appointment.Id).ToList().Count, Is.EqualTo(2));
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        foundAppointment = (_appointmentStorage.FindBy(x => x.Id == appointment.Id).First());
        var foundPrescription1 = _prescriptionStorage.FindBy(x => x.Id == prescription1.Id).First();
        var foundPrescription2 = _prescriptionStorage.FindBy(x => x.Id == prescription2.Id).First();
        Assert.That(foundAppointment?.Prescriptions.Count, Is.EqualTo(2));
        Assert.IsTrue(foundAppointment?.Prescriptions.ContainsKey(foundPrescription1.Id));
        Assert.IsTrue(foundAppointment?.Prescriptions.ContainsKey(foundPrescription2.Id));
        Assert.That(_prescriptionStorage
            .FindBy(x => x.IdAppointment == appointment.Id).ToList().Count, Is.EqualTo(2));
    }
    
    [Test, Category("Equipment"), Category("Serialize")]
    public void EquipmentSerializeTest()
    {
        var equipment1 = _equipmentFactory.CreateNewEquipment("IV stand", DateTime.Today);
        var equipment2 = _equipmentFactory.CreateNewEquipment("IV stand Pro", DateTime.Today);
        var room = _roomFactory.CreateNewRoom(3);
        
        room.Equipments.Add(equipment1);
        equipment1.Room = room;
        room.Equipments.Add(equipment2);
        equipment2.Room = room;
        
        var foundRoom = (_roomStorage.FindBy(x => x.Id == room.Id).First());
        Assert.That(foundRoom?.Equipments.Count, Is.EqualTo(2));
        Assert.That(_equipmentStorage
            .FindBy(x => x.IdRoom == room.Id).ToList().Count, Is.EqualTo(2));
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        foundRoom = (_roomStorage.FindBy(x => x.Id == room.Id).First());
        Assert.That(foundRoom?.Equipments.Count, Is.EqualTo(2));
        Assert.That(_equipmentStorage
            .FindBy(x => x.IdRoom == room.Id).ToList().Count, Is.EqualTo(2));
    }
    
    [Test, Category("RoomPatient"), Category("Serialize")]
    public void RoomPatientSerializeTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var room = _roomFactory.CreateNewRoom(3);
        var roomPatient = _roomPatientFactory.CreateNewRoomPatient(room, patient, DateTime.Now, null);
        
        var foundRoom = (_roomStorage.FindBy(x => x.Id == room.Id).First());
        var foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        Assert.That(foundRoom?.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(foundPatient?.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage
            .FindBy(x => x.IdRoom == room.Id && x.IdPatient == patient.Id).ToList().Count, Is.EqualTo(1));
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        foundRoom = (_roomStorage.FindBy(x => x.Id == room.Id).First());
        foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        Assert.That(foundRoom?.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(foundPatient?.RoomPatients.Count, Is.EqualTo(1));
        Assert.That(_roomPatientStorage
            .FindBy(x => x.IdRoom == room.Id && x.IdPatient == patient.Id).ToList().Count, Is.EqualTo(1));
    }

    [Test, Category("Diagnosis"), Category("Serialize")]
    public void DiagnosisSerializeTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereAnxiety
        (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null,
            LevelOfDanger.High, true);
        
        var foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(_diagnosisStorage.FindBy(x => x.IdPatient == patient.Id).ToList().Count, Is.EqualTo(2));
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
         foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(_diagnosisStorage.FindBy(x => x.IdPatient == patient.Id).ToList().Count, Is.EqualTo(2));
    }
    
    [Test, Category("Serialize")]
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
    
    [Test, Category("Serialize")]
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
        thersp.Patients.Add(patient);
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
        
       
        var foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
        var foundTherapist = (_personStorage.FindBy(x => x.Id == thersp.Id).First() as Therapist);
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(foundPatient?.Therapists.Count , Is.EqualTo(1));
        Assert.That(foundTherapist?.Patients.Count , Is.EqualTo(1));
        var foundNurse2 = (_personStorage.FindBy(x => x.Id == nurse2.Id).First() as Nurse);
        Assert.That(foundTherapist?.Subordinates.Count , Is.EqualTo(1));
        Assert.That(foundNurse2?.Supervisor , !Is.Null);
        var foundNurse = (_personStorage.FindBy(x => x.Id == nurse.Id).First() as Nurse);
        Assert.That(foundNurse?.Rooms.Count , Is.EqualTo(1));
        var foundRoom = _roomStorage.FindBy(x => x.Id == room.Id).First();
        Assert.That(foundRoom?.Nurses.Count , Is.EqualTo(1));
        var foundAppoint = _appointmentStorage.FindBy(x => x.Id == appoint.Id).First();
        Assert.That(foundAppoint?.Therapist.Id , Is.EqualTo(foundTherapist?.Id));
        Assert.That(foundAppoint?.Patient?.Id , Is.EqualTo(foundPatient?.Id));
        var foundPrescr = _prescriptionStorage.FindBy(x => x.Id == prescr.Id).First();
        Assert.That(foundPrescr?.IdAppointment , Is.EqualTo(foundAppoint?.Id));
     
        
        _storageManager.Serialize();
        _storageManager.Deserialize();
        
        
         foundPatient = (_personStorage.FindBy(x => x.Id == patient.Id).First() as Patient);
         foundTherapist = (_personStorage.FindBy(x => x.Id == thersp.Id).First() as Therapist);
        
        Assert.That(foundPatient?.Diagnoses.Count, Is.EqualTo(2));
        Assert.That(foundPatient?.Therapists.Count , Is.EqualTo(1));
        Assert.That(foundTherapist?.Patients.Count , Is.EqualTo(1));
         foundNurse2 = (_personStorage.FindBy(x => x.Id == nurse2.Id).First() as Nurse);
        Assert.That(foundTherapist?.Subordinates.Count , Is.EqualTo(1));
        Assert.That(foundNurse2?.Supervisor , !Is.Null);
         foundNurse = (_personStorage.FindBy(x => x.Id == nurse.Id).First() as Nurse);
        Assert.That(foundNurse?.Rooms.Count , Is.EqualTo(1));
         foundRoom = _roomStorage.FindBy(x => x.Id == room.Id).First();
        Assert.That(foundRoom?.Nurses.Count , Is.EqualTo(1));
         foundAppoint = _appointmentStorage.FindBy(x => x.Id == appoint.Id).First();
        Assert.That(foundAppoint?.Therapist.Id , Is.EqualTo(foundTherapist?.Id));
        Assert.That(foundAppoint?.Patient?.Id , Is.EqualTo(foundPatient?.Id));
         foundPrescr = _prescriptionStorage.FindBy(x => x.Id == prescr.Id).First();
        Assert.That(foundPrescr?.IdAppointment , Is.EqualTo(foundAppoint?.Id));
        
    }


    [Test, Category("Association")]
    public void DeleteDiagnosisFromPatientAssociationTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereAnxiety
        (patient, "anexity", "severe cases of bad luck in the past", new string[0], DateTime.Now, null,
            LevelOfDanger.High, true);
        Assert.That(patient.Diagnoses.Count , Is.EqualTo(2));

        patient.Diagnoses.Remove(diagnosis2);
        Assert.That(patient.Diagnoses.Count , Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x.Id == diagnosis2.Id) , Is.Empty);
        
    }
}