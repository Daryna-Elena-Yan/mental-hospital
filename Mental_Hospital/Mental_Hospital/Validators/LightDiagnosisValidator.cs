using FluentValidation;
using Mental_Hospital.Models.Light;

namespace Mental_Hospital.Validators;

public class LightDiagnosisValidator : AbstractValidator<Light>
{
    public LightDiagnosisValidator(DiagnosisValidator diagnosisValidator)
    {
        Include(diagnosisValidator);
        RuleFor(x => x.IsSupervisionRequired).NotNull().WithMessage("Supervision restrictions must be specified");
    }
}