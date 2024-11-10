using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class DiagnosisFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Diagnosis> _diagnoses;
    private readonly LightDiagnosisValidator _lightDiagnosisValidator;
    private readonly SevereDiagnosisValidator _severeDiagnosisValidator;

    public DiagnosisFactory(IServiceProvider provider, Storage<Diagnosis> diagnoses, 
        LightDiagnosisValidator lightDiagnosisValidator, SevereDiagnosisValidator severeDiagnosisValidator)
    {
        _provider = provider;
        _diagnoses = diagnoses;
        _lightDiagnosisValidator = lightDiagnosisValidator;
        _severeDiagnosisValidator = severeDiagnosisValidator;
    }


    public LightAnxiety CreateNewLightAnxiety(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> triggers, DateTime dateOfDiagnosis, DateTime? dateOfHealing,  bool isSupervisionRequired)
    {
        var lightAnxiety = _provider.GetRequiredService<LightAnxiety>();
        foreach (var trigger in triggers)
        {
            lightAnxiety.Triggers.Add(trigger);
        }
        SetLight(lightAnxiety, isSupervisionRequired);
        SetDiagnosis(lightAnxiety, patient, nameOfDisorder,description,dateOfDiagnosis,dateOfHealing);
        _lightDiagnosisValidator.ValidateAndThrow(lightAnxiety); 
        return lightAnxiety;
    }
    
    public SevereAnxiety CreateNewSevereAnxiety(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> triggers, DateTime dateOfDiagnosis, DateTime? dateOfHealing,
        LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    {
        var severeAnxiety = _provider.GetRequiredService<SevereAnxiety>();
        foreach (var trigger in triggers)
        {
            severeAnxiety.Triggers.Add(trigger);
        }
        SetSevere(severeAnxiety, levelOfDanger, isPhysicalRestraintRequired);
        SetDiagnosis(severeAnxiety, patient, nameOfDisorder,description,dateOfDiagnosis,dateOfHealing);
        _severeDiagnosisValidator.ValidateAndThrow(severeAnxiety); 
        return severeAnxiety;
    }
    
    public LightMood CreateNewLightMood(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> consumedPsychedelics, DateTime dateOfDiagnosis, DateTime? dateOfHealing, bool isSupervisionRequired)
    {
        var lightMood = _provider.GetRequiredService<LightMood>();
        foreach (var psycodel in consumedPsychedelics)
        {
            lightMood.ConsumedPsychedelics.Add(psycodel);
        }
        SetLight(lightMood, isSupervisionRequired);
        SetDiagnosis(lightMood, patient, nameOfDisorder,description,dateOfDiagnosis,dateOfHealing);
        _lightDiagnosisValidator.ValidateAndThrow(lightMood);
        return lightMood;
    }
    
 
    
    public SevereMood CreateNewSevereMood(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> consumedPsychedelics, DateTime dateOfDiagnosis, DateTime? dateOfHealing,
        LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    { 
        var severeMood = _provider.GetRequiredService<SevereMood>();
        
        foreach (var psycodel in consumedPsychedelics)
        {
            severeMood.ConsumedPsychedelics.Add(psycodel);
        }
        SetSevere(severeMood, levelOfDanger, isPhysicalRestraintRequired);
        SetDiagnosis(severeMood, patient, nameOfDisorder,description,dateOfDiagnosis,dateOfHealing);
        _severeDiagnosisValidator.ValidateAndThrow(severeMood);
        return severeMood;
    }
    
    public LightPsychotic CreateNewLightPsychotic(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> hallucinations, DateTime dateOfDiagnosis, DateTime? dateOfHealing, bool isSupervisionRequired)
    {
       var lightPsychotic = _provider.GetRequiredService<LightPsychotic>();
        foreach (var hallucination in hallucinations)
        {
            lightPsychotic.Hallucinations.Add(hallucination);
        }
        SetLight(lightPsychotic, isSupervisionRequired);
        SetDiagnosis(lightPsychotic, patient, nameOfDisorder,description,dateOfDiagnosis,dateOfHealing);
        _lightDiagnosisValidator.ValidateAndThrow(lightPsychotic);
        return lightPsychotic;
    }
    
    public SeverePsychotic CreateNewSeverePsychotic(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> hallucinations, DateTime dateOfDiagnosis, DateTime? dateOfHealing,
        LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    {
        var severePsychotic = _provider.GetRequiredService<SeverePsychotic>();
        foreach (var hallucination in hallucinations)
        {
            severePsychotic.Hallucinations.Add(hallucination);
        }
        SetSevere(severePsychotic, levelOfDanger, isPhysicalRestraintRequired);
        SetDiagnosis(severePsychotic, patient, nameOfDisorder,description,dateOfDiagnosis,dateOfHealing);
        _severeDiagnosisValidator.ValidateAndThrow(severePsychotic);
        return severePsychotic;
    }
    private void SetDiagnosis(Diagnosis diagnosis, Patient patient, string nameOfDisorder, string description,
        DateTime dateOfDiagnosis, DateTime? dateOfHealing)
    {
        diagnosis.IdDisorder = Guid.NewGuid();
        diagnosis.Description = description;
        diagnosis.NameOfDisorder = nameOfDisorder;
        diagnosis.DateOfDiagnosis = dateOfDiagnosis;
        diagnosis.DateOfHealing = dateOfHealing;
        diagnosis.Patient = patient;
        
        
        _diagnoses.RegisterNew(diagnosis);
    }

    private void SetSevere(Severe severe, LevelOfDanger levelOfDanger, bool isPhysicalRestraintRequired)
    {
        severe.LevelOfDanger = levelOfDanger;
        severe.IsPhysicalRestraintRequired = isPhysicalRestraintRequired;
    }
    
    private void SetLight(Light light,  bool isSupervisionRequired)
    {
        light.IsSupervisionRequired = isSupervisionRequired;
    }
    
}