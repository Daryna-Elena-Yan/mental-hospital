namespace Mental_Hospital.Models;

public class Therapist:Employee
{
    public static double BasicSalaryInZl = 10000;
    public static double OvertimePaidPerHourInZl=70;
    public List<string> Qualifications { get; set; }
    public virtual ICollection<Appointment> Appointments { get; } = [];
    public virtual ICollection<Patient> Patients{ get; }= new List<Patient>();
    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
}