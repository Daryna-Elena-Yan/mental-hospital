using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.Name).MinimumLength(2).WithMessage("Name should ba at least 2 characters long.");
        RuleFor(x => x.Surname).MinimumLength(2).WithMessage("Surname should ba at least 2 characters long.");
        RuleFor(x => x.Address).Length(10, 70).WithMessage("Address must be of length from 10 to 70 symbols.");
        RuleFor(x => x.DateOfBirth).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of birth");
    }
}