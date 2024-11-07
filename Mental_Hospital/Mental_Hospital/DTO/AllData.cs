using System.Text.Json.Serialization;
using Mental_Hospital.Models;

namespace Mental_Hospital.DTO;

public class AllData
{
    public List<Patient> Patients { get; set; } = new List<Patient>();
    public List<Nurse> Nurses { get; set; } = new List<Nurse>();
    public List<Therapist> Therapists { get; set; } = new List<Therapist>();
    public List<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
    public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    public List<Equipment> Equipments { get; set; } = new List<Equipment>();
    public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public List<Room> Rooms { get; set; } = new List<Room>();
    public List<RoomPatient> RoomPatients { get; set; } = new List<RoomPatient>();
    [JsonConstructor]
    public AllData() { }
}
