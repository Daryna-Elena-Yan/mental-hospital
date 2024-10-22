using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class PersonFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Person> _storage;

    public PersonFactory(IServiceProvider provider, Storage<Person> storage)
    {
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
}