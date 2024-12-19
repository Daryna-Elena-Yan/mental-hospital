using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Equipment : IEntity
{
    private Room? _room;
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid? IdRoom{ get; set;  }

    [JsonIgnore]
    public virtual Room? Room
    {
        get => _room;
        set
        {
            IdRoom = value?.Id;
            _room = value;
            if(value != null)
                if(value.Equipments!=null)
                    if (!value.Equipments.Contains(this))
                        value.Equipments.Add(this);
        }
    }
}