namespace Mental_Hospital.Models;

public class Nurse:Employee
{
    public static double BasicSalaryInZl { get; } = 6000;
    public static double OvertimePaidPerHourInZl { get; } = 50;
    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }

    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
}