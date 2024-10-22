using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class Room_PatientFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Room_Patient> _storage;

    public Room_PatientFactory(IServiceProvider provider, Storage<Room_Patient> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Room_Patient CreateNewRoomPatient(DateTime datePlaced, DateTime? dateDischarged)
    {
        var roomPatient = _provider.GetRequiredService<Room_Patient>();
        roomPatient.DatePlaced = datePlaced;
        roomPatient.DateDischarged = dateDischarged;
        
        _storage.RegisterNew(roomPatient);
        
        return roomPatient;
    }
}