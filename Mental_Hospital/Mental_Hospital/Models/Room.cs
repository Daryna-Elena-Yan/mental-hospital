using System.Text.Json.Serialization;
using Mental_Hospital.Collections;

namespace Mental_Hospital.Models;

public class Room : IEntity
{
    public Guid Id { get; set; } 
    public int Capacity { get; set; }
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Nurse>))]
    public virtual AssociationCollection<Nurse> Nurses { get; set; } 
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Equipment>))]
    public virtual AssociationCollection<Equipment> Equipments { get; set; }
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<RoomPatient>))]
    public virtual AssociationCollection<RoomPatient> RoomPatients { get; set; } 
}