using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Room : IEntity
{
    [JsonConstructor]
    public Room()
    {
    }

    public Guid IdRoom { get; set; } 
    public int Capacity { get; set; }
    [JsonIgnore]

    public virtual ICollection<Nurse> Nurses { get; } = [];
    [JsonIgnore]

    public virtual ICollection<Equipment> Equipments { get; } =[];
    [JsonIgnore]

    public virtual ICollection<RoomPatient> RoomPatients { get; } = [];
}