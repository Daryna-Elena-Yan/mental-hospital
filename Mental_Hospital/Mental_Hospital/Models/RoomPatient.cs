using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class RoomPatient
{
    [JsonConstructor]
    public RoomPatient()
    {
    }

    public DateTime DatePlaced { get; set; }
    public DateTime? DateDischarged { get; set; }
    public Guid? IdRoom{ get; set;  }
    public Guid? IdPatient{ get; set;  }
    [JsonIgnore]

    public virtual Room Room { get; set; } = null!;
    [JsonIgnore]

    public virtual Patient Patient { get; set;  } = null!;
}