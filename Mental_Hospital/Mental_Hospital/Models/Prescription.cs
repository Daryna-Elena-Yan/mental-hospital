using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Prescription : IEntity
{
    [JsonConstructor]
    public Prescription()
    {
    }

    public Guid IdPrescription { get; set; } 
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Decimal Dosage { get; set; }
    public string Description { get; set; }
    public Guid? IdAppointment{ get; set;  }
    [JsonIgnore]

    public virtual Appointment? Appointment { get; set;  }
}