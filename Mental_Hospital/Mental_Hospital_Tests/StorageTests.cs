using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital_Tests;

public class Tests
{
    private PersonFactory _personFactory;
    private Storage<Person> _personStorage;
    private DiagnosisFactory _diagnosisFactory;
    private Storage<Diagnosis> _diagnosisStorage;
    private Storage<PatientDiagnosis> _patientDiagnosisStorage;


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
        
        
        _personStorage = provider.GetRequiredService<Storage<Person>>();
        _diagnosisStorage = provider.GetRequiredService<Storage<Diagnosis>>();
        _patientDiagnosisStorage = provider.GetRequiredService<Storage<PatientDiagnosis>>();
    }

    [Test]
    public void PersonStorageRegisterTest()
    {
        _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        Assert.That(_personStorage.Count, Is.EqualTo(1));
    }

    
    [Test]
    public void PatientStorageDeleteTest()
    {
      var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
      var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
          (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
      
      Assert.That(_personStorage.Count, Is.EqualTo(1));
      Assert.That(_diagnosisStorage.Count, Is.EqualTo(1));
      Assert.That(_patientDiagnosisStorage.Count, Is.EqualTo(1));
      
      
      _personStorage.Delete(patient);
      
      Assert.That(_personStorage.Count, Is.EqualTo(0));
      Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
      Assert.That(_patientDiagnosisStorage.Count, Is.EqualTo(0));

    }
}