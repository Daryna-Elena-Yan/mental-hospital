using System.Data;
using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Validators;

public class RoomPatientValidator : AbstractValidator<RoomPatient>
{
    private Storage<Patient> _patientStorage;
    private Storage<Room> _roomStorage;
    public RoomPatientValidator(Storage<Patient> patientStorage, Storage<Room> roomStorage)
    {
        _patientStorage = patientStorage;
        _roomStorage = roomStorage;
        RuleFor(x => x.DatePlaced).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of being placed");
        RuleFor(x => new { x.DatePlaced, x.DateDischarged })
            .Must(x => IsPlacedEarlierThanDischarged(x.DatePlaced, x.DateDischarged))
            .WithMessage("Date of being placed cannot be earlier that date of being discharged.");
        RuleFor(x => x.Patient).NotNull().Must(DoesPatientExist).WithMessage("Patient does not exist.");
        RuleFor(x => x.Room).NotNull().Must(DoesRoomExist).WithMessage("Room does not exist.");

    }
    private bool IsPlacedEarlierThanDischarged(DateTime datePlaced, DateTime? dateDischarged)
    {
        if (!dateDischarged.HasValue) return true;
        return dateDischarged.Value.CompareTo(datePlaced) > 0;
    }
    private bool DoesRoomExist(Room room )
    {
        return _roomStorage.FindBy(x => x.IdRoom == room.IdRoom).Any();
    } 
    private bool DoesPatientExist(Patient patient)
    {
        return _patientStorage.FindBy(x => x.IdPerson == patient.IdPerson).Any();
    } 
}