using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital_Tests;

public class Tests
{
    private PersonFactory? _factory;
    private Storage<Person>? _personStorage;
    private Storage<Patient>? _patientStorage;

    [SetUp]
    public void Setup()
    {
        //Initiallizing DI container
        var services = new ServiceCollection();
        services.MentalHospitalSetup();
        var provider = services.BuildServiceProvider();

        //getting of services instances for tests
        _factory = provider.GetService<PersonFactory>();
        
        _personStorage = provider.GetService<Storage<Person>>();
        _patientStorage = provider.GetService<Storage<Patient>>();
    }

    [Test]
    public void PersonStorageRegisterTest()
    {
        _factory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
    }

    public void PatientStorageDeleteTest()
    {
        //TODO
      var patient =  _factory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        //patient.PatientDiagnoses.Add();
        
        
    }
}