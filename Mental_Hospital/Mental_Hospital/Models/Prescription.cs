using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace Mental_Hospital.Models;

public class Prescription
{
    public int IdPrescription { get; set; } // identifier i.e. qualified value
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Decimal Dosage { get; set; }
    public string Description { get; set; }
}