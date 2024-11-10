using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class EquipmentValidator : AbstractValidator<Equipment>
{
    public EquipmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Specify name of the Equipment.");
        RuleFor(x => x.ExpirationDate).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of expiration");
    }
}