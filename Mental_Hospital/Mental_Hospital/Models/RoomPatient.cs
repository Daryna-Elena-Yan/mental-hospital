using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class RoomPatient : IEntity
{
    private Room _room = null!;
    private Patient _patient = null!;

    public DateTime DatePlaced { get; set; }
    public DateTime? DateDischarged { get; set; }
    public Guid? IdRoom{ get; set;  }
    public Guid? IdPatient{ get; set;  }

    [JsonIgnore]
    public virtual Room Room
    {
        get => _room;
        set
        {
            IdRoom = value.IdRoom;
            _room = value;
        }
    }

    [JsonIgnore]
    public virtual Patient Patient
    {
        get => _patient;
        set
        {
            IdPatient = value.IdPerson;
            _patient = value;
        }
    }
}