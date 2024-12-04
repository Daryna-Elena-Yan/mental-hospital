using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public class Nurse:Employee
{


    public new static double BasicSalaryInZl { get; } = 6000;
    
    public new static double OvertimePaidPerHourInZl { get; } = 50;
    public ICollection<Guid> IdsRooms{ get; set;  }=[];

    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
    
    [JsonIgnore]
    public virtual ICollection<Room> Rooms { get; } = [];

    public void AddRoom(Room room)
    {
        Rooms.Add(room);
        IdsRooms.Add(room.Id);
    }
}