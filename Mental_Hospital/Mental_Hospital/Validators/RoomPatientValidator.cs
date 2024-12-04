using System.Data;
using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;

namespace Mental_Hospital.Validators;

public class RoomPatientValidator : AbstractValidator<RoomPatient>
{
    private readonly Storage<Person> _personStorage;
    private readonly Storage<Room> _roomStorage;
    public RoomPatientValidator(Storage<Person> personStorage, Storage<Room> roomStorage)
    {
        _personStorage = personStorage;
        _roomStorage = roomStorage;
        RuleFor(x => x.DatePlaced).NotNull()
            .Must(x => x != DateTime.MinValue)
            .WithMessage("Specify date of being placed.");
        RuleFor(x => new { x.DatePlaced, x.DateDischarged })
            .Must(x => IsPlacedEarlierThanDischarged(x.DatePlaced, x.DateDischarged))
            .WithMessage("Date of being placed cannot be earlier that date of being discharged.");
        RuleFor(x => x.Patient).NotNull().WithMessage(("Patient is required."))
            .Must(DoesPatientExist).WithMessage("Patient does not exist.");
        RuleFor(x => x.Room).NotNull().NotNull().WithMessage(("Room is required."))
            .Must(DoesRoomExist).WithMessage("Room does not exist.");

    }
    private bool IsPlacedEarlierThanDischarged(DateTime datePlaced, DateTime? dateDischarged)
    {
        if (!dateDischarged.HasValue) return true;
        return dateDischarged.Value.CompareTo(datePlaced) > 0;
    }
    private bool DoesRoomExist(Room? room )
    {
        return room is null ? true : _roomStorage.FindBy(x => x.Id == room.Id).Any();
    } 
    private bool DoesPatientExist(Patient? patient)
    {
        return patient is null ? true : _personStorage.FindBy(x => x.Id == patient.Id).Any();
    } 
}