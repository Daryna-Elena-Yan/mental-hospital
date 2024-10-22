namespace Mental_Hospital.Models;

public class Appointment
{
    public int IdAppointment { get; set; }
    public DateTime DateOfAppointment { get; set; }
    public string Description { get; set; }
    public virtual Patient Patient { get; } = null!;
    // TODO: HashMap for Prescriptions
}