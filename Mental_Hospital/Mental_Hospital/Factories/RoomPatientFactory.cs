using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class RoomPatientFactory : IFactory
{ 
    private readonly IServiceProvider _provider;
    private readonly Storage<RoomPatient> _storage;
    private readonly RoomPatientValidator _validator;

    public RoomPatientFactory(IServiceProvider provider, Storage<RoomPatient> storage, RoomPatientValidator validator)
    {
        _provider = provider;
        _storage = storage;
        _validator = validator;
    }

    public RoomPatient CreateNewRoomPatient(Room room, Patient patient, DateTime datePlaced, DateTime? dateDischarged)
    {
        var roomPatient = _provider.GetRequiredService<RoomPatient>();
        roomPatient.DatePlaced = datePlaced;
        roomPatient.DateDischarged = dateDischarged;
        roomPatient.Patient = patient;
        roomPatient.Room = room;
        
        _validator.ValidateAndThrow(roomPatient);
        _storage.RegisterNew(roomPatient);
        
        return roomPatient;
    }
}