﻿using System.Text.Json.Serialization;
using Mental_Hospital.Collections;

namespace Mental_Hospital.Models;

public class Patient : Person
{
    public string Anamnesis { get; set; }
    public DateTime? DateOfDeath { get; set; }

    [JsonIgnore]
    public virtual AssociationCollection<Diagnosis> Diagnoses { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; } = [];
    
    [JsonIgnore]
    public virtual ICollection<RoomPatient> RoomPatients { get; } = [];
    
    [JsonIgnore]
    public virtual ICollection<Therapist> Therapists{ get; }= [];
    
    
}