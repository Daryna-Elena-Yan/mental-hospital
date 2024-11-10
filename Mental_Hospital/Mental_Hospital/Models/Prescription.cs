using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Prescription : IEntity
{
    private Appointment? _appointment;
    

    public Guid IdPrescription { get; set; }  // identifier i.e. qualified value
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Decimal Dosage { get; set; }
    public string Description { get; set; }
    public Guid? IdAppointment{ get; set;  }

    [JsonIgnore]
    public virtual Appointment? Appointment
    {
        get => _appointment;
        set
        {
            IdAppointment = value?.IdAppointment;
            _appointment = value;
        }
    }
}