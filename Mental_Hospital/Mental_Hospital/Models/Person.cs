using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public abstract class Person
{
    [JsonConstructor]
    protected Person()
    {
    }

    public Guid IdPerson { get; set; }
    public string Name{ get; set; }
    public string Surname{ get; set; }
    public DateTime DateOfBirth{ get; set; }
    public string Address{ get; set; }
    
}