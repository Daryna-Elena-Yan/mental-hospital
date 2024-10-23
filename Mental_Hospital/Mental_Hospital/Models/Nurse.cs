namespace Mental_Hospital.Models;

public class Nurse:Employee
{
    public static double BasicSalaryInZl { get; } = 6000;
    public static double OvertimePaidPerHourInZl { get; } = 50;
}