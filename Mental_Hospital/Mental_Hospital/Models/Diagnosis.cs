namespace Mental_Hospital.Models;

public abstract class Diagnosis
{
    public Guid IdDisorder { get; } = Guid.NewGuid();
    public virtual string NameOfDisorder { get; set; }
    public virtual string Description { get; set; }
    public virtual PatientDiagnosis PatientDiagnosis { get; set; }
}