namespace Mental_Hospital.Models;

public class Patient : Person
{
    public string Anamnesis { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public virtual ICollection<Appointment> Appointments { get; } = new List<Appointment>();
    public virtual ICollection<Room_Patient> RoomPatients { get; } = new List<Room_Patient>();
}