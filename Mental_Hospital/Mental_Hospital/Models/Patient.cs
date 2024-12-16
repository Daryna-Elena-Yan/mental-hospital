using System.Text.Json.Serialization;
using Mental_Hospital.Collections;

namespace Mental_Hospital.Models;

public class Patient : Person
{
    public string Anamnesis { get; set; }
    public DateTime? DateOfDeath { get; set; }

    [JsonConverter(typeof(AssociationCollectionJsonConverter<Diagnosis>))]
    public virtual AssociationCollection<Diagnosis> Diagnoses { get; set; }
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Appointment>))]
    public virtual AssociationCollection<Appointment> Appointments { get; set; }
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<RoomPatient>))]
    public virtual AssociationCollection<RoomPatient> RoomPatients { get; set; } 
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Therapist>))]
    public virtual AssociationCollection<Therapist> Therapists{ get; set;}
    
    
}