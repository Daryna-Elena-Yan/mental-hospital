using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Nurse:Employee
{
    [JsonConstructor]
    public Nurse()
    {
    }

    public static double BasicSalaryInZl { get; } = 6000;
    public static double OvertimePaidPerHourInZl { get; } = 50;
    public List<Guid> IdsRooms{ get; set;  }=[];

    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
    [JsonIgnore]

    public virtual ICollection<Room> Rooms { get; } = [];
}