using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class PrescriptionValidator : AbstractValidator<Prescription>
{
    public PrescriptionValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Specify name of the Prescription.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dosage).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).Length(20, 500).WithMessage("Description should be from 20 to 500 symbols long.");
    }
}