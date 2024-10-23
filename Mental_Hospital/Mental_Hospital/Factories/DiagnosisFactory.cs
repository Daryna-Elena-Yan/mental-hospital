using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class DiagnosisFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Diagnosis> _diagnoses;

    public DiagnosisFactory(IServiceProvider provider,
        Storage<Diagnosis> diagnoses)
    {
        _provider = provider;
        _diagnoses = diagnoses;
    }


    public LightAnxiety CreateNewLightAnxiety(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> triggers, DateTime dateOfDiagnosis, DateTime? dateOfHealing)
    {
        // TODO check if patient exists, if not return null 
        
        var lightAnxiety = _provider.GetRequiredService<LightAnxiety>();
        lightAnxiety.Description = description;
        lightAnxiety.NameOfDisorder = nameOfDisorder;
        foreach (var trigger in triggers)
        {
            lightAnxiety.Triggers.Add(trigger);
        }
        lightAnxiety.DateOfDiagnosis = dateOfDiagnosis;
        lightAnxiety.DateOfHealing = dateOfHealing;
        lightAnxiety.Patient = patient;
        
        patient.Diagnoses.Add(lightAnxiety);
        
        _diagnoses.RegisterNew(lightAnxiety);

        return lightAnxiety;
    }
    
    public SevereAnxiety CreateNewSevereAnxiety(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> triggers, DateTime dateOfDiagnosis, DateTime? dateOfHealing,
        LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    {
        // TODO check if patient exists, if not return null 
        
        var severeAnxiety = _provider.GetRequiredService<SevereAnxiety>();
        severeAnxiety.Description = description;
        severeAnxiety.NameOfDisorder = nameOfDisorder;
        foreach (var trigger in triggers)
        {
            severeAnxiety.Triggers.Add(trigger);
        }
        severeAnxiety.DateOfDiagnosis = dateOfDiagnosis;
        severeAnxiety.DateOfHealing = dateOfHealing;
        severeAnxiety.LevelOfDanger = levelOfDanger;
        severeAnxiety.IsPhysicalRestraintRequired = isPhysicalRestraintRequired;
        severeAnxiety.Patient = patient;
        
        patient.Diagnoses.Add(severeAnxiety);
        
        _diagnoses.RegisterNew(severeAnxiety);

        return severeAnxiety;
    }
    
    public LightMood CreateNewLightMood(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> consumedPsychedelics, DateTime dateOfDiagnosis, DateTime? dateOfHealing)
    {
        // TODO check if patient exists, if not return null 
        
        var lightMood = _provider.GetRequiredService<LightMood>();
        lightMood.Description = description;
        lightMood.NameOfDisorder = nameOfDisorder;
        foreach (var psycodel in consumedPsychedelics)
        {
            lightMood.ConsumedPsychedelics.Add(psycodel);
        }
        lightMood.DateOfDiagnosis = dateOfDiagnosis;
        lightMood.DateOfHealing = dateOfHealing;
        lightMood.Patient = patient;
        
        patient.Diagnoses.Add(lightMood);
        
        _diagnoses.RegisterNew(lightMood);

        return lightMood;
    }
    
    public SevereMood CreateNewSevereMood(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> consumedPsychedelics, DateTime dateOfDiagnosis, DateTime? dateOfHealing,
        LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    {
        // TODO check if patient exists, if not return null 
        
        var severeMood = _provider.GetRequiredService<SevereMood>();
        severeMood.Description = description;
        severeMood.NameOfDisorder = nameOfDisorder;
        foreach (var psycodel in consumedPsychedelics)
        {
            severeMood.ConsumedPsychedelics.Add(psycodel);
        }
        severeMood.DateOfDiagnosis = dateOfDiagnosis;
        severeMood.DateOfHealing = dateOfHealing;
        severeMood.LevelOfDanger = levelOfDanger;
        severeMood.IsPhysicalRestraintRequired = isPhysicalRestraintRequired;
        severeMood.Patient = patient;
        
        patient.Diagnoses.Add(severeMood);
        
        _diagnoses.RegisterNew(severeMood);

        return severeMood;
    }
    
    public SeverePsychotic CreateNewSeverePsychotic(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> hallucinations, DateTime dateOfDiagnosis, DateTime? dateOfHealing,
        LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    {
        // TODO check if patient exists, if not return null 
        
        var severePsychotic = _provider.GetRequiredService<SeverePsychotic>();
        severePsychotic.Description = description;
        severePsychotic.NameOfDisorder = nameOfDisorder;
        foreach (var hal in hallucinations)
        {
            severePsychotic.Hallucinations.Add(hal);
        }
        severePsychotic.DateOfDiagnosis = dateOfDiagnosis;
        severePsychotic.DateOfHealing = dateOfHealing;
        severePsychotic.LevelOfDanger = levelOfDanger;
        severePsychotic.IsPhysicalRestraintRequired = isPhysicalRestraintRequired;
        severePsychotic.Patient = patient;
        
        patient.Diagnoses.Add(severePsychotic);
        
        _diagnoses.RegisterNew(severePsychotic);

        return severePsychotic;
    }
    
    public LightPsychotic CreateNewLightPsychotic(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> hallucinations, DateTime dateOfDiagnosis, DateTime? dateOfHealing)
    {
        // TODO check if patient exists, if not return null 
        
        var lightPsychotic = _provider.GetRequiredService<LightPsychotic>();
        lightPsychotic.Description = description;
        lightPsychotic.NameOfDisorder = nameOfDisorder;
        foreach (var hal in hallucinations)
        {
            lightPsychotic.Hallucinations.Add(hal);
        }
        lightPsychotic.DateOfDiagnosis = dateOfDiagnosis;
        lightPsychotic.DateOfHealing = dateOfHealing;
        lightPsychotic.Patient = patient;
        
        patient.Diagnoses.Add(lightPsychotic);
        
        _diagnoses.RegisterNew(lightPsychotic);

        return lightPsychotic;
    }
    
}