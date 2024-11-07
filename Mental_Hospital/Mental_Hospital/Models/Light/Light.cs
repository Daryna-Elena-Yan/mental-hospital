namespace Mental_Hospital.Models.Light;

public abstract class Light : Diagnosis
{
    public bool IsSupervisionRequired { get; set; }
}