namespace Mental_Hospital.Models;

public class Therapist:Employee
{
    public static double BasicSalaryInZl = 10000;
    public static double OvertimePaidPerHourInZl=70;
    public List<string> Qualifications { get; set; }
    public virtual ICollection<Appointment> Appointments { get; } = [];
}