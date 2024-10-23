namespace Mental_Hospital.Models;

public class Patient : Person
{
    public string Anamnesis { get; set; }
    public DateTime? DateOfDeath { get; set; }
    public virtual ICollection<Diagnosis> Diagnoses { get; } = [];
    public virtual ICollection<Appointment> Appointments { get; } = [];
    public virtual ICollection<RoomPatient> RoomPatients { get; } = [];
    public virtual ICollection<Therapist> Therapists{ get; }= new List<Therapist>();

}