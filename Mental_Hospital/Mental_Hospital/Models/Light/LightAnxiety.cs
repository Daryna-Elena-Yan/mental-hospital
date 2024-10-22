namespace Mental_Hospital.Models.Light;

public class LightAnxiety : Light
{
    public ICollection<string> Triggers { get; } = [];
}