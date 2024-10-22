namespace Mental_Hospital.Models;

public abstract class Person
{
    public int IdPerson { get; set; }
    public string Name{ get; set; }
    public string Surname{ get; set; }
    public DateTime DateOfBirth{ get; set; }
    public string Address{ get; set; }
    
}