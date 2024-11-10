using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator(PersonValidator personValidator)
    {
        Include(personValidator);
        RuleFor(x => new { x.DateOfBirth, x.DateOfDeath })
            .Must(x => IsBirthEarlierThanDeath( x.DateOfBirth, x.DateOfDeath))
            .WithMessage("Date of healing cannot be earlier that date of Diagnosing.");
    }

    private bool IsBirthEarlierThanDeath(DateTime dateOfBirth, DateTime? dateOfDeath)
    {
        if (!dateOfDeath.HasValue) return true;
        return dateOfDeath.Value.CompareTo(dateOfBirth) > 0;
    }
}