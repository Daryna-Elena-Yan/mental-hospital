﻿using System.Text.Json.Serialization;
using Microsoft.VisualBasic.CompilerServices;

namespace Mental_Hospital.Models;

public class Appointment
{
    [JsonConstructor]
    public Appointment()
    {
    }

    public Guid IdAppointment { get; set; } 
    public DateTime DateOfAppointment { get; set; }
    public string Description { get; set; }
    public Guid? IdPatient{ get; set;  }
    public Guid? IdTherapist{ get; set;  }
    [JsonIgnore]
    public virtual Patient? Patient { get; set;  }
    [JsonIgnore]

    public virtual Therapist Therapist { get; set;  } = null!;
    [JsonIgnore]

    public Dictionary<int, Prescription> Prescriptions = new ();
}