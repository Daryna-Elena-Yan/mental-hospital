using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class RoomPatient  : IEntity
//TODO another type of , not IEntity 
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
            IdRoom = value?.Id;
            _room = value;
            if(value != null)
                if(value.RoomPatients!=null)
                    if (!value.RoomPatients.Contains(this))
                        value.RoomPatients.Add(this);
        }
    }

    [JsonIgnore]
    public virtual Patient Patient
    {
        get => _patient;
        set
        {
            IdPatient = value?.Id;
            _patient = value;
            if(value != null)
                if(value.RoomPatients!=null)
                    if (!value.RoomPatients.Contains(this))
                        value.RoomPatients.Add(this);
        }
    }

    public Guid Id { get; } = new Guid();
}