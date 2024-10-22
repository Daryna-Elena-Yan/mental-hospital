namespace Mental_Hospital.Models;

public class Appointment
{
    public Guid IdAppointment { get; } = Guid.NewGuid();
    public DateTime DateOfAppointment { get; set; }
    public string Description { get; set; }
    public virtual Patient Patient { get; } = null!;
    // TODO: HashMap for Prescriptions
}