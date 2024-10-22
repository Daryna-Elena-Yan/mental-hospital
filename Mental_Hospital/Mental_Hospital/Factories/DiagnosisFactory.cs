using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class DiagnosisFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Diagnosis> _diagnoses;
    private readonly Storage<PatientDiagnosis> _patientDiagnosis;

    public DiagnosisFactory(IServiceProvider provider,
        Storage<Diagnosis> diagnoses, Storage<PatientDiagnosis> patientDiagnosis)
    {
        _provider = provider;
        _diagnoses = diagnoses;
        _patientDiagnosis = patientDiagnosis;
    }


    public LightAnxiety CreateNewDiagnosis(Patient patient, string nameOfDisorder, string description,
        IEnumerable<string> triggers, DateTime dateOfDiagnosis, DateTime? dateOfHealing)
    {
        var lightAnxiety = _provider.GetRequiredService<LightAnxiety>();
        lightAnxiety.Description = description;
        lightAnxiety.NameOfDisorder = nameOfDisorder;

        foreach (var trigger in triggers)
        {
            lightAnxiety.Triggers.Add(trigger);
        }

        var patientDiagnosis = _provider.GetRequiredService<PatientDiagnosis>();
        patientDiagnosis.Diagnosis = lightAnxiety;
        patientDiagnosis.Patient = patient;
        patientDiagnosis.DateOfDiagnosis = dateOfDiagnosis;
        patientDiagnosis.DateOfHealing = dateOfHealing;

        lightAnxiety.PatientDiagnosis = patientDiagnosis;
        patient.PatientDiagnoses.Add(patientDiagnosis);
        
        _diagnoses.RegisterNew(lightAnxiety);
        _patientDiagnosis.RegisterNew(patientDiagnosis);

        return lightAnxiety;
    }
}