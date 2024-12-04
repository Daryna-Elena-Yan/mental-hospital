using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    private readonly Storage<Person> _personStorage;

    public EmployeeValidator(PersonValidator personValidator,Storage<Person> personStorage)
    {
        _personStorage = personStorage;
        Include(personValidator);
        RuleFor(x => x.Bonus).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OvertimePerMonth).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(
            x => x is Nurse ? Nurse.BasicSalaryInZl : Therapist.BasicSalaryInZl);
        RuleFor(x => x.DateHired).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of being hired");
        RuleFor(x => new { x.DateHired, x.DateFired })
            .Must(x => IsHireEarlierThanFire(x.DateHired, x.DateFired))
            .WithMessage("Date of being fired cannot be earlier that date of being hired.");
        RuleFor(x => x.Supervisor).Must(DoesEmployeeExist).WithMessage("No such employee found.");
    }

    private bool IsHireEarlierThanFire(DateTime dateOfHired, DateTime? dateOfFired)
    {
        if (!dateOfFired.HasValue) return true;
        return dateOfFired.Value.CompareTo(dateOfHired) > 0;
    }
    private bool DoesEmployeeExist(Employee? employee)
    {
        return employee is null ? true : _personStorage.FindBy(x => x.Id == employee.Id).Any();
    } 
}