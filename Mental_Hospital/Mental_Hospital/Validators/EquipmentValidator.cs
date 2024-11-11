using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class EquipmentValidator : AbstractValidator<Equipment>
{
    public EquipmentValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage("Please specify name.")
            .MinimumLength(1).WithMessage("Name should be at least 1 characters long.");
        RuleFor(x => x.ExpirationDate).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of expiration.");
    }
}