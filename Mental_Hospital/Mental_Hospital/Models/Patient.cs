namespace Mental_Hospital.Models;

public class Patient : Person
{
    public string Anamnesis { get; set; }
    public DateTime? DateOfDeath { get; set; }
    
}