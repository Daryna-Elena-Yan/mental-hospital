using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public abstract class Diagnosis
{
    [JsonConstructor]
    protected Diagnosis()
    {
    }

    public Guid IdDisorder { get; set; } 
    public virtual string NameOfDisorder { get; set; }
    public virtual string Description { get; set; }
    public DateTime DateOfDiagnosis { get; set; }
    public DateTime? DateOfHealing { get; set; }
    public Guid? IdPatient{ get; set;  }
    [JsonIgnore]

    public virtual Patient Patient { get; set; }
}