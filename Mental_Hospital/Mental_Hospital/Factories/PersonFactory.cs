using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class PersonFactory {
    private readonly IServiceProvider _provider;
    private readonly Storage<Person> _storage;

    public PersonFactory(IServiceProvider provider, Storage<Person> storage) {
        _provider = provider;
        _storage = storage;
    }

    public Patient CreateNewPatient(string name, string surname, DateTime dateOfBirth, string address, 
        string anamnesis, DateTime? dateOfDeath)
    {
        var patient = _provider.GetRequiredService<Patient>(); //to create inside DI container
        patient.Name = name;
        patient.Surname = surname;
        patient.DateOfDeath = dateOfBirth;
        patient.Address = address;
        patient.Anamnesis = anamnesis;
        patient.DateOfDeath = dateOfDeath;

        _storage.RegisterNew(patient);
        
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
        nurse.DateHired=DateTime.Now;
        nurse.DateOfBirth = dateOfBirth;
        nurse.Salary=nurse.Bonus+Nurse.BasicSalaryInZl+nurse.OvertimePerMonth*Nurse.OvertimePaidPerHourInZl;
        _storage.RegisterNew(nurse);
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
        therapist.DateHired=DateTime.Now;
        therapist.DateOfBirth = dateOfBirth;
        therapist.Salary=therapist.Bonus+Therapist.BasicSalaryInZl+therapist.OvertimePerMonth*Therapist.OvertimePaidPerHourInZl;
        therapist.Qualifications = qualifications;
        _storage.RegisterNew(therapist);
        return therapist;
    }
}