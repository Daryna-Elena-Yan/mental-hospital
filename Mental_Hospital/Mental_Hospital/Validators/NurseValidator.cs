using FluentValidation;
using Mental_Hospital.Models;

namespace Mental_Hospital.Validators;

public class NurseValidator : AbstractValidator<Nurse>
{
    public NurseValidator(EmployeeValidator employeeValidator)
    {
        Include(employeeValidator);
    }
}