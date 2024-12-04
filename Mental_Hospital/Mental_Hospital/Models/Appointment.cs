using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices;

namespace Mental_Hospital.Models;

public class Appointment : IEntity
{
    public Guid Id { get; set; } 
    public DateTime DateOfAppointment { get; set; }
    public string Description { get; set; }
    public Guid? IdPatient { get; set;  }
    public Guid? IdTherapist { get; set;  }

    [JsonIgnore]
    public virtual Patient? Patient
    {
        get => _patient;
        set
        {
            IdPatient = value?.Id;
            _patient = value;
        }
    }

    [JsonIgnore]
    public virtual Therapist Therapist
    {
        get => _therapist;
        set
        {
            IdTherapist = value.Id;
            _therapist = value;
        }
    }

    [JsonIgnore]
    public Dictionary<Guid, Prescription> Prescriptions { get; set; } = new ();

    private Patient? _patient;
    private Therapist _therapist = null!;
}