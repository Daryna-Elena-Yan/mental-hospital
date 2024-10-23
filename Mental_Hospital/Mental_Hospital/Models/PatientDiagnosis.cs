namespace Mental_Hospital.Models;

public class PatientDiagnosis
{
    public virtual Patient Patient { get; set; }
    public virtual Diagnosis Diagnosis { get; set; }
    //TODO delte!!!
    public DateTime DateOfDiagnosis { get; set; }
    public DateTime? DateOfHealing { get; set; }
    
}