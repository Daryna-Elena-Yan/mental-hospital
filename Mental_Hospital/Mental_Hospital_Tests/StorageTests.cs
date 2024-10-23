using Mental_Hospital;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital_Tests;

public class Tests
{
    private PersonFactory _personFactory;
    private Storage<Person> _personStorage;
    private DiagnosisFactory _diagnosisFactory;
    private Storage<Diagnosis> _diagnosisStorage;


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
      Assert.That(patient.Diagnoses.Count, Is.EqualTo(1));
      Assert.That(diagnosis.Patient, !Is.Null);
      
      _personStorage.Delete(patient);
      
      Assert.That(_personStorage.Count, Is.EqualTo(0));
      Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
    }
    
    
    
    
    [Test]
    public void AddNewDiagnosisTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        
        Assert.That(_personStorage.Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.Count, Is.EqualTo(1));
        Assert.That(patient.Diagnoses.Where(x => x == diagnosis).Count, Is.EqualTo(1));
        
    }
    
    [Test]
    public void DeleteDiagnosisTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        
        _diagnosisStorage.Delete(diagnosis);
        
        Assert.That(_diagnosisStorage.Count, Is.EqualTo(0));
        Assert.That(patient.Diagnoses.Where(x => x == diagnosis).Count, Is.EqualTo(0));
        
    }
    
    
    
    [Test]
    public void AllDiagnosisTypeCreationTest()
    {
        var patient =  _personFactory.CreateNewPatient("Charles", "Leclerc", DateTime.Now,
            "Baker Street, 221B", "Depression", null);
        var diagnosis = _diagnosisFactory.CreateNewLightAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        var diagnosis4 = _diagnosisFactory.CreateNewLightMood
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        var diagnosis5 = _diagnosisFactory.CreateNewLightPsychotic
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null);
        var diagnosis1 = _diagnosisFactory.CreateNewSevereAnxiety
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null, LevelOfDanger.High, true);
        var diagnosis2 = _diagnosisFactory.CreateNewSevereMood
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null, LevelOfDanger.High, true);
        var diagnosis3 = _diagnosisFactory.CreateNewSeverePsychotic
            (patient, "anexity", "aaaaa", new string[0], DateTime.Now, null, LevelOfDanger.High, true);

        
        Assert.That(_diagnosisStorage.FindBy(x => x is SevereMood) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is SeverePsychotic) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is SevereAnxiety) .Count, Is.EqualTo(1));
        
        Assert.That(_diagnosisStorage.FindBy(x => x is LightMood) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is LightPsychotic) .Count, Is.EqualTo(1));
        Assert.That(_diagnosisStorage.FindBy(x => x is LightAnxiety) .Count, Is.EqualTo(1));
        
    }
    
}