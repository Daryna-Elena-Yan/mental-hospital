using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage("Name cannot be null.").MinimumLength(2).WithMessage("Name should be at least 2 characters long.");
        RuleFor(x => x.Surname).NotNull().WithMessage("Surname cannot be null.").MinimumLength(2).WithMessage("Surname should be at least 2 characters long.");
        RuleFor(x => x.Address).NotNull().WithMessage("Address cannot be null.").Length(10, 70).WithMessage("Address must be of length from 10 to 70 symbols.");
        RuleFor(x => x.DateOfBirth).NotNull().WithMessage("Date of birth cannot be null.")
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of birth");
    }
}