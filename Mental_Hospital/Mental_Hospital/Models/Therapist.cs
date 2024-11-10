using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Therapist:Employee
{
    [JsonConstructor]
    public Therapist()
    {
    }

    public static double BasicSalaryInZl = 10000;
    public static double OvertimePaidPerHourInZl=70;
    public List<string> Qualifications { get; } = [];
    public List<Guid> IdsPatients{ get; set;  }= [];
    [JsonIgnore]

    public virtual ICollection<Appointment> Appointments { get; } = [];
    [JsonIgnore]

    public virtual ICollection<Patient> Patients{ get; }=[];
    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
}