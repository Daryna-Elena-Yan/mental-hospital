using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital_Tests;

public class Tests
{
    private PersonFactory? _factory;
    private Storage<Person>? _storage;

    [SetUp]
    public void Setup()
    {
        //Initiallizing DI container
        var services = new ServiceCollection();
        services.MentalHospitalSetup();
        var provider = services.BuildServiceProvider();


        _factory = provider.GetService<PersonFactory>();
        _storage = provider.GetService<Storage<Person>>();
    }

    [Test]
    public void PersonStorageRegisterTest()
    {
        _factory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        Assert.That(_storage.Count, Is.EqualTo(1));
    }
}