using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;


[JsonDerivedType(typeof(Patient), typeDiscriminator: nameof(Patient))]
[JsonDerivedType(typeof(Nurse), typeDiscriminator: nameof(Nurse))]
[JsonDerivedType(typeof(Therapist), typeDiscriminator: nameof(Therapist))]
public abstract class Person : IEntity
{
    public Guid IdPerson { get; set; }
    public string Name{ get; set; }
    public string Surname{ get; set; }
    public DateTime DateOfBirth{ get; set; }
    public string Address{ get; set; }
    
    
    
    
}