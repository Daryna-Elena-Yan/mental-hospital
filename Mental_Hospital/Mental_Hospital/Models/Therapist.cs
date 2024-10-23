namespace Mental_Hospital.Models;

public class Therapist:Employee
{
    private static double _basicSalaryInZl = 10000;
    private static double _overtimePaidPerHourInZl=70;
    private List<string> _qualifications { get; set; }
}