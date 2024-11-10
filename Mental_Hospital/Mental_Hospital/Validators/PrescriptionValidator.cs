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
        RuleFor(x => x.Name).NotEmpty().WithMessage("Specify name of the Prescription.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dosage).GreaterThanOrEqualTo(0);
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