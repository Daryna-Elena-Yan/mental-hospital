using FluentValidation;
using Mental_Hospital.Models.Severe;

namespace Mental_Hospital.Validators;

public class SevereDiagnosisValidator : AbstractValidator<Severe>
{
    public SevereDiagnosisValidator(DiagnosisValidator diagnosisValidator)
    {
        Include(diagnosisValidator);
        RuleFor(x => x.LevelOfDanger).NotNull().WithMessage("Level of danger must be specified");
        RuleFor(x => x.IsPhysicalRestraintRequired).NotNull().WithMessage("Requirement of physical restrictions must be specified");
    }
}