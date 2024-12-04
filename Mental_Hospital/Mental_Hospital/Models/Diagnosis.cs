using System.Text.Json.Serialization;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;

namespace Mental_Hospital.Models;



[JsonDerivedType(typeof(LightAnxiety), typeDiscriminator: nameof(LightAnxiety))]
[JsonDerivedType(typeof(SevereAnxiety), typeDiscriminator: nameof(SevereAnxiety))]
[JsonDerivedType(typeof(LightMood), typeDiscriminator: nameof(LightMood))]
[JsonDerivedType(typeof(SevereMood), typeDiscriminator: nameof(SevereMood))]
[JsonDerivedType(typeof(LightPsychotic), typeDiscriminator: nameof(LightPsychotic))]
[JsonDerivedType(typeof(SeverePsychotic), typeDiscriminator: nameof(SeverePsychotic))]
public abstract class Diagnosis : IEntity
{
    private Patient _patient;
    

    public Guid Id { get; set; } 
    public virtual string NameOfDisorder { get; set; }
    public virtual string Description { get; set; }
    public DateTime DateOfDiagnosis { get; set; }
    public DateTime? DateOfHealing { get; set; }
    public Guid? IdPatient{ get; set; }
    

    [JsonIgnore]
    public virtual Patient Patient
    {
        get => _patient;
        set
        {
            IdPatient = value?.Id;
            _patient = value;
        }
    }
}