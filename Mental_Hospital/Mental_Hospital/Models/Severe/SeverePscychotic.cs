namespace Mental_Hospital.Models.Severe;

public class SeverePsychotic : Severe
{
    public ICollection<string> Hallucinations { get; } = []; 
}