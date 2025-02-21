﻿using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Prescription : IEntity
{
    private Appointment? _appointment;
    

    public Guid Id { get; set; } 
    public string Name { get; set; }
    public int Quantity { get; set; }
    public Decimal Dosage { get; set; }
    public string Description { get; set; }
    public Guid? IdAppointment{ get; set;  }

    [JsonIgnore]
    public virtual Appointment? Appointment
    {
        get => _appointment;
        set
        {
            IdAppointment = value?.Id;
            _appointment = value;
            if(value != null)
                if(value.Prescriptions!=null)
                    if (!value.Prescriptions.Contains(new KeyValuePair<Guid, Prescription>(Id,this)))
                        value.Prescriptions.Add(Id,this);
        }
    }

}