using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Validators;

public class PrescriptionValidator : AbstractValidator<Prescription>
{
    private readonly Storage<Appointment> _appointmentStorage;
    public PrescriptionValidator(Storage<Appointment> appointmentStorage)
    {
        _appointmentStorage = appointmentStorage;
        RuleFor(x => x.Name).MinimumLength(1).WithMessage("Name should be at least 1 characters long.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0.");
        RuleFor(x => x.Dosage).GreaterThanOrEqualTo(0).WithMessage("Dosage must be greater than or equal to 0.");
        RuleFor(x => x.Description).Length(20, 500).WithMessage("Description should be from 20 to 500 symbols long.");
        RuleFor(x => x.Appointment).Custom((appointment, context) => {
            if (appointment != null) {
                if (!DoesAppointmentExist(appointment))
                {
                    context.AddFailure( "Appointment does not exist.");
                }
            }
        });
    }
    private bool DoesAppointmentExist(Appointment appointment)
    {
        return _appointmentStorage.FindBy(x => x.IdAppointment == appointment.IdAppointment).Any();
    } 
}