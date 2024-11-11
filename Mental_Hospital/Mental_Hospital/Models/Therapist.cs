using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Therapist : Employee
{
    
    public new static double BasicSalaryInZl = 10000;
    public new static double OvertimePaidPerHourInZl=70;
    
    public List<string> Qualifications { get; set; } = [];
    public ICollection<Guid> IdsPatients{ get; set; }= [];
    
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = [];
    
    [JsonIgnore]
    public virtual ICollection<Patient> Patients { get; set; } =[];
    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
    
    public void AddPatient(Patient patient)
    {
        Patients.Add(patient);
        IdsPatients.Add(patient.IdPerson);
    }
}