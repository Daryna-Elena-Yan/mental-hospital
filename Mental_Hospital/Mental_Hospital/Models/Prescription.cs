namespace Mental_Hospital.Models;

public class Prescription
{
    public Guid IdPrescription { get;  } = Guid.NewGuid(); // identifier i.e. qualified value
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Decimal Dosage { get; set; }
    public string Description { get; set; }
    public virtual Appointment? Appointment { get; set;  } = null!;
}