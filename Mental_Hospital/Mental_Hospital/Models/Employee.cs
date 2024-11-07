﻿using System.Text.Json.Serialization;

namespace Mental_Hospital.Models;

public abstract class Employee :Person
{
    [JsonConstructor]
    protected Employee()
    {
    }

    public double Bonus { get; set; }
    public static double BasicSalaryInZl { get; set; }
    public static double OvertimePaidPerHourInZl { get; set; }
    public double OvertimePerMonth { get; set; }
    public double Salary { get; set; }
    public DateTime DateHired { get; set; }
    public DateTime? DateFired { get; set; }
    public Guid? IdSupervisor{ get; set;  }
    [JsonIgnore]

    public virtual ICollection<Employee> Subordinates{ get; }= [];
    [JsonIgnore]

    public virtual Employee? Supervisor{ get; set; }

    public abstract void RecalculateSalary();
}