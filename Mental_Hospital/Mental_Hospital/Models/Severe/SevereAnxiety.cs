namespace Mental_Hospital.Models.Severe;

public class SevereAnxiety : Severe
{
    public ICollection<string> Triggers { get; set;} = [];
}