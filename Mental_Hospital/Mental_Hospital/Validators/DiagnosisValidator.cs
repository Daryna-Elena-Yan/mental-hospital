using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Validators;

public class DiagnosisValidator : AbstractValidator<Diagnosis>
{
    private readonly Storage<Person> _personStorage;

    public DiagnosisValidator(Storage<Person> personStorage)
    {
        _personStorage = personStorage;
        RuleFor(x => x.NameOfDisorder).NotEmpty().WithMessage("Specify name of the Diagnosis.");
        RuleFor(x => x.DateOfDiagnosis).NotNull()
            .Must(x => x != DateTime.MinValue).WithMessage("Specify date of diagnosis.");
        RuleFor(x => new { x.DateOfHealing, x.DateOfDiagnosis })
            .Must(x => IsHealingDateAfterDiagnosing(x.DateOfHealing, x.DateOfDiagnosis))
            .WithMessage("Date of healing cannot be earlier that date of Diagnosing.");
        RuleFor(x => x.Description).Length(20, 500).WithMessage("Description should be from 20 to 500 symbols long.");
        RuleFor(x => x.Patient).Must(DoesPatientExist).NotNull();
    }

    private bool DoesPatientExist(Patient patient)
    {
       return _personStorage.FindBy(x => x.IdPerson == patient.IdPerson).Any();
    }

    private bool IsHealingDateAfterDiagnosing(DateTime? healing, DateTime diagnosing)
    {
        if (!healing.HasValue) return true;
        return healing.Value.CompareTo(diagnosing) > 0;
    }
}