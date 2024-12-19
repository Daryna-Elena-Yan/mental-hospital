using System.Text.Json.Serialization;
using Mental_Hospital.Collections;

namespace Mental_Hospital.Models;

public abstract class Employee :Person
{
    private Employee? _supervisor;
    
    public double Bonus { get; set; }
    public static double BasicSalaryInZl { get; set; }
    public static double OvertimePaidPerHourInZl { get; set; }
    public double OvertimePerMonth { get; set; }
    public double Salary { get; set; }
    public DateTime DateHired { get; set; }
    public DateTime? DateFired { get; set; }
    public Guid? IdSupervisor{ get; set;  }
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Employee>))]
    public virtual AssociationCollection<Employee> Subordinates { get; set; }

    [JsonIgnore]
    public virtual Employee? Supervisor
    {
        get => _supervisor;
        set
        {
            IdSupervisor = value?.Id;
            _supervisor = value;
            if(value != null)
                if(value.Subordinates!=null)
                    if (!value.Subordinates.Contains(this))
                        value.Subordinates.Add(this);
        }
    }

    public abstract void RecalculateSalary();
}