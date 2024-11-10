using System.Data;
using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Validators;

public class AppointmentValidator : AbstractValidator<Appointment>
{
    private Storage<Therapist> _therapistStorage;
    private Storage<Patient> _patientStorage;
    
    public AppointmentValidator(Storage<Therapist> therapistStorage, Storage<Patient> patientStorage)
    {
        _therapistStorage = therapistStorage;
        _patientStorage = patientStorage;
        RuleFor(x => x.DateOfAppointment).NotNull()
            .Must(x => x != DateTime.MinValue).WithMessage("Specify date of appointment.");;
        RuleFor(x => x.Description).Length(20, 500).WithMessage("Description should be from 20 to 500 symbols long.");
        RuleFor(x => x.Therapist).NotNull().Must(DoesTherapistExist).WithMessage("Therapist does not exist.");
        RuleFor(x => x.Patient).Custom((patient, context) => {
                if (patient != null) {
                    if (!DoesPatientExist(patient))
                    {
                        context.AddFailure( "Patient does not exist.");
                    }
                }
        });
    }
    private bool DoesTherapistExist(Therapist therapist)
    {
        return _therapistStorage.FindBy(x => x.IdPerson == therapist.IdPerson).Any();
    } 
    private bool DoesPatientExist(Patient patient)
    {
        return _patientStorage.FindBy(x => x.IdPerson == patient.IdPerson).Any();
    } 
}