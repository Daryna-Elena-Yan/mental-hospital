using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class PersonFactory {
    private readonly IServiceProvider _provider;
    private readonly Storage<Therapist> _therapisStorage;
    private readonly Storage<Patient> _patientStorage;
    private readonly Storage<Nurse> _nurseStorage;
    private readonly PatientValidator _patientValidator;

    public PersonFactory(IServiceProvider provider, Storage<Therapist> tstorage,Storage<Nurse> nstorage,Storage<Patient> pstorage, PatientValidator patientValidator) {
        _provider = provider;
        _therapisStorage = tstorage;
        _patientStorage = pstorage;
        _nurseStorage = nstorage;
        _patientValidator = patientValidator;
    }

    public Patient CreateNewPatient(string name, string surname, DateTime dateOfBirth, string address, 
        string anamnesis, DateTime? dateOfDeath)
    {
        var patient = _provider.GetRequiredService<Patient>(); //to create inside DI container
        patient.IdPerson = Guid.NewGuid();
        patient.Name = name;
        patient.Surname = surname;
        patient.DateOfBirth = dateOfBirth;
        patient.Address = address;
        patient.Anamnesis = anamnesis;
        patient.DateOfDeath = dateOfDeath;
        _patientValidator.ValidateAndThrow(patient);
        _patientStorage.RegisterNew(patient);
        
        return patient;
    }
    public Nurse CreateNewNurse(Employee? supervisor, string name, string surname, DateTime dateOfBirth, string address)
    {
        var nurse = _provider.GetRequiredService<Nurse>();
        nurse.Name = name;
        nurse.Surname = surname;
        nurse.Bonus = 0;
        nurse.OvertimePerMonth = 0;
        nurse.Address = address;
        nurse.Supervisor = supervisor;
        nurse.DateFired = null;
        nurse.DateHired=DateTime.Today;
        nurse.DateOfBirth = dateOfBirth;
        nurse.Salary=nurse.Bonus+Nurse.BasicSalaryInZl+nurse.OvertimePerMonth*Nurse.OvertimePaidPerHourInZl;
        _nurseStorage.RegisterNew(nurse);
        return nurse;
    }
    public Therapist CreateNewTherapist(Therapist? supervisor, string name, string surname, DateTime dateOfBirth, string address, 
        List<string> qualifications)
    {
        var therapist = _provider.GetRequiredService<Therapist>();
        therapist.Name = name;
        therapist.Surname = surname;
        therapist.Bonus = 0;
        therapist.OvertimePerMonth = 0;
        therapist.Address = address;
        therapist.Supervisor = supervisor;
        therapist.DateFired = null;
        therapist.DateHired=DateTime.Today;
        therapist.DateOfBirth = dateOfBirth;
        therapist.Salary=therapist.Bonus+Therapist.BasicSalaryInZl+therapist.OvertimePerMonth*Therapist.OvertimePaidPerHourInZl;
        therapist.Qualifications = qualifications;
        _therapisStorage.RegisterNew(therapist);
        return therapist;
    }
}