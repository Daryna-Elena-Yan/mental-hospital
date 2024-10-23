namespace Mental_Hospital.Models;

public abstract class Employee :Person
{
    private double _bonus { get; set; }
    private static double _basicSalaryInZl{ get; set; }
    private static double _overtimePaidPerHourInZl{ get; set; }
    private Employee? _supervisor{ get; set; }
    private double _overtimePerMonth { get; set; }
    private double _salary { get; set; }
    private DateTime _dateHired { get; set; }
    private DateTime? _dateFired { get; set; }
}