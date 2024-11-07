using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class DiagnosisValidator : AbstractValidator<Diagnosis>
{
    public DiagnosisValidator(PatientValidator patientValidator)
    {
        RuleFor(x => x.NameOfDisorder).NotEmpty().WithMessage("Specify name of the Diagnosis.");
        RuleFor(x => x.DateOfDiagnosis).NotNull()
            .Must(x => x != DateTime.MinValue).WithMessage("Specify date of diagnosis.");
        RuleFor(x => new { x.DateOfHealing, x.DateOfDiagnosis })
            .Must(x => IsHealingDateAfterDiagnosing(x.DateOfHealing, x.DateOfDiagnosis))
            .WithMessage("Date of healing cannot be earlier that date of Diagnosing.");
        RuleFor(x => x.Description).Length(20, 500).WithMessage("Description should be from 20 to 500 symbols long.");
        RuleFor(x => x.Patient).SetValidator(patientValidator).NotNull();
    }

    private bool IsHealingDateAfterDiagnosing(DateTime? healing, DateTime diagnosing)
    {
        if (!healing.HasValue) return true;
        return healing.Value.CompareTo(diagnosing) > 0;
    }
}