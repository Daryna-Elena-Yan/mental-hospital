namespace Mental_Hospital.Models.Severe;

public class SevereMood : Severe
{
    public ICollection<string> ConsumedPsychedelics { get; } = [];
}