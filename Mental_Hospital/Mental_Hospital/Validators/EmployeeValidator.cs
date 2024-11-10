using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator(PersonValidator personValidator)
    {
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
    }

    private bool IsHireEarlierThanFire(DateTime dateOfHired, DateTime? dateOfFired)
    {
        if (!dateOfFired.HasValue) return true;
        return dateOfFired.Value.CompareTo(dateOfHired) > 0;
    }
}