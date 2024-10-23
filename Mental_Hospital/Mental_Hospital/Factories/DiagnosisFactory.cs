using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
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
}