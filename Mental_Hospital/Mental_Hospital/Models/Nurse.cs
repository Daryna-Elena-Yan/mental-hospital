namespace Mental_Hospital.Models;

public class Nurse:Employee
{
    public static double BasicSalaryInZl { get; } = 6000;
    public static double OvertimePaidPerHourInZl { get; } = 50;
    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
}