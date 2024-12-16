using FluentValidation;
using Mental_Hospital.Collections;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class PersonFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Person> _personStorage;
    private readonly PatientValidator _patientValidator;
    private readonly TherapistValidator _therapistValidator;
    private readonly NurseValidator _nurseValidator;


    public PersonFactory(IServiceProvider provider, PatientValidator patientValidator,
        TherapistValidator therapistValidator, NurseValidator nurseValidator, Storage<Person> personStorage)
    {
        _provider = provider;
        _patientValidator = patientValidator;
        _personStorage = personStorage;
        _therapistValidator = therapistValidator;
        _nurseValidator = nurseValidator;
    }

    public Patient CreateNewPatient(string name, string surname, DateTime dateOfBirth, string address,
        string anamnesis, DateTime? dateOfDeath)
    {
        var patient = _provider.GetRequiredService<Patient>(); //to create inside DI container
        patient.Id = Guid.NewGuid();
        patient.Name = name;
        patient.Surname = surname;
        patient.DateOfBirth = dateOfBirth;
        patient.Address = address;
        patient.Anamnesis = anamnesis;
        patient.DateOfDeath = dateOfDeath;
        patient.Diagnoses = new AssociationCollection<Diagnosis>(patient, _provider);
        patient.Appointments = new AssociationCollection<Appointment>(patient, _provider);
        patient.RoomPatients = new AssociationCollection<RoomPatient>(patient, _provider);
        patient.Therapists = new AssociationCollection<Therapist>(patient, _provider);
        _patientValidator.ValidateAndThrow(patient);
        _personStorage.RegisterNew(patient);

        return patient;
    }

    public Nurse CreateNewNurse(Employee? supervisor, string name, string surname, DateTime dateOfBirth, string address)
    {
        var nurse = _provider.GetRequiredService<Nurse>();
        nurse.Id = Guid.NewGuid();
        nurse.Name = name;
        nurse.Surname = surname;
        nurse.Bonus = 0;
        nurse.OvertimePerMonth = 0;
        nurse.Address = address;
        nurse.Supervisor = supervisor;
        nurse.Subordinates = new AssociationCollection<Employee>(nurse, _provider);
        nurse.Rooms = new AssociationCollection<Room>(nurse, _provider);
        nurse.DateFired = null;
        nurse.DateHired = DateTime.Today;
        nurse.DateOfBirth = dateOfBirth;
        nurse.Salary = nurse.Bonus + Nurse.BasicSalaryInZl + nurse.OvertimePerMonth * Nurse.OvertimePaidPerHourInZl;
        _nurseValidator.ValidateAndThrow(nurse);
        _personStorage.RegisterNew(nurse);
        return nurse;
    }

    public Therapist CreateNewTherapist(Therapist? supervisor, string name, string surname, DateTime dateOfBirth,
        string address,
        IEnumerable<string> qualifications)
    {
        var therapist = _provider.GetRequiredService<Therapist>();
        therapist.Id = Guid.NewGuid();
        therapist.Name = name;
        therapist.Surname = surname;
        therapist.Bonus = 0;
        therapist.OvertimePerMonth = 0;
        therapist.Address = address;
        therapist.Supervisor = supervisor;
        therapist.Subordinates = new AssociationCollection<Employee>(therapist, _provider);
        therapist.Appointments = new AssociationCollection<Appointment>(therapist, _provider);
        therapist.DateFired = null;
        therapist.DateHired = DateTime.Today;
        therapist.DateOfBirth = dateOfBirth;
        therapist.Salary = therapist.Bonus + Therapist.BasicSalaryInZl +
                           therapist.OvertimePerMonth * Therapist.OvertimePaidPerHourInZl;
        if (qualifications is not null)
        {
            foreach (var q in qualifications)
            {
                therapist.Qualifications.Add(q);
            }
        }

        therapist.Patients = new AssociationCollection<Patient>(therapist, _provider);

        _therapistValidator.ValidateAndThrow(therapist);
        _personStorage.RegisterNew(therapist);
        return therapist;
    }
}