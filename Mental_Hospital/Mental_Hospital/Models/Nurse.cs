using System.Text.Json.Serialization;
using Mental_Hospital.Collections;

namespace Mental_Hospital.Models;

public class Nurse:Employee
{


    public new static double BasicSalaryInZl { get; } = 6000;
    
    public new static double OvertimePaidPerHourInZl { get; } = 50;
    //public ICollection<Guid> IdsRooms{ get; set;  }=[];

    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Room>))]
    public virtual AssociationCollection<Room> Rooms { get; set; }
    
}