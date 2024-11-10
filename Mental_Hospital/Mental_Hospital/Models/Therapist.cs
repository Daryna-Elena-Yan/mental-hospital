using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Therapist:Employee
{
    
    public new static double BasicSalaryInZl = 10000;
    public new static double OvertimePaidPerHourInZl=70;
    public ICollection<string> Qualifications { get; } = [];
    public ICollection<Guid> IdsPatients{ get; }= [];
    
    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; } = [];
    
    [JsonIgnore]
    public virtual ICollection<Patient> Patients{ get; }=[];
    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
}