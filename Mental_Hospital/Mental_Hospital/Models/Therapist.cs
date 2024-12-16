using System.Text.Json.Serialization;
using Mental_Hospital.Collections;

namespace Mental_Hospital.Models;

public class Therapist : Employee
{
    
    public new static double BasicSalaryInZl = 10000;
    public new static double OvertimePaidPerHourInZl=70;
    
    public List<string> Qualifications { get; set; } = [];
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Appointment>))]
    public virtual AssociationCollection<Appointment> Appointments { get; set; }
    
    [JsonConverter(typeof(AssociationCollectionJsonConverter<Patient>))]
    public AssociationCollection<Patient> Patients { get; set; }
    public override void RecalculateSalary()
    {
        this.Salary=this.Bonus+BasicSalaryInZl+this.OvertimePerMonth*OvertimePaidPerHourInZl;
    }
}