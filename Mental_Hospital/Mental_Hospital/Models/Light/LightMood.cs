namespace Mental_Hospital.Models.Light;

public class LightMood : Light
{
    public ICollection<string> ConsumedPsychedelics { get; } = [];
}