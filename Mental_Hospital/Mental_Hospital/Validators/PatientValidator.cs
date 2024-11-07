using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name should not be empty.");
        RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname should not be empty.");
        RuleFor(x => x.DateOfBirth).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of birth");
        RuleFor(x => new { x.DateOfBirth, x.DateOfDeath })
            .Must(x => IsBirthEarlierThanDeath( x.DateOfBirth, x.DateOfDeath))
            .WithMessage("Date of healing cannot be earlier that date of Diagnosing.");
        RuleFor(x => x.Address).Length(10, 70).WithMessage("Address must be of length from 10 to 70 symbols.");
    }

    private bool IsBirthEarlierThanDeath(DateTime dateOfBirth, DateTime? dateOfDeath)
    {
        if (!dateOfDeath.HasValue) return true;
        return dateOfDeath.Value.CompareTo(dateOfBirth) > 0;
    }
}