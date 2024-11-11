namespace Mental_Hospital.Models.Light;

public class LightPsychotic : Light
{
    public ICollection<string> Hallucinations { get; set;} = []; 
}