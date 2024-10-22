namespace Mental_Hospital.Models.Severe;

public abstract class Severe : Diagnosis
{
    public virtual LevelOfDanger LevelOfDanger { get; set; }
    public virtual bool IsPhysicalRestraintRequired { get; set; }
}

public enum LevelOfDanger
{
    Low, Medium, High
}