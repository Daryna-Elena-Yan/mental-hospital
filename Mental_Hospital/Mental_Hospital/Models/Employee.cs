namespace Mental_Hospital.Models;

public abstract class Employee :Person
{
    public double Bonus { get; set; }
    public Employee? Supervisor{ get; set; }
    public double OvertimePerMonth { get; set; }
    public double Salary { get; set; }
    public DateTime DateHired { get; set; }
    public DateTime? DateFired { get; set; }
}