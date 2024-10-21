using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital_Tests;

public class Tests
{
    private PersonFactory? _factory;
    private PersonStorage? _storage;

    [SetUp]
    public void Setup()
    {
        //Initiallizing DI container
        var services = new ServiceCollection();
        services.MentalHospitalSetup();
        var provider = services.BuildServiceProvider();


        _factory = provider.GetService<PersonFactory>();
        _storage = provider.GetService<PersonStorage>();
    }

    [Test]
    public void PersonStorageRegisterTest()
    {
        _factory.CreateNewPatient("aa", "aa", DateTime.Now, "asssa", "asads");
        Assert.That(_storage.Count, Is.EqualTo(1));
    }
}