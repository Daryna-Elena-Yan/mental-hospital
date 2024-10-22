namespace Mental_Hospital.Models;

public abstract class Person
{
    public Guid IdPerson { get; } = Guid.NewGuid();
    public string Name{ get; set; }
    public string Surname{ get; set; }
    public DateTime DateOfBirth{ get; set; }
    public string Address{ get; set; }
    
}