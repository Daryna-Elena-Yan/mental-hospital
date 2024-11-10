using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class TherapistValidator : AbstractValidator<Therapist>
{
    public TherapistValidator(EmployeeValidator employeeValidator)
    {
        Include(employeeValidator);
        RuleFor(x => x.Qualifications).NotEmpty().WithMessage("Therapist must have at least one qualification.");
        RuleFor(x => x.Supervisor).Must(x => x is Therapist);
    }
}