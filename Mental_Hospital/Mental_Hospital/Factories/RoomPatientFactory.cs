using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class RoomPatientFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<RoomPatient> _storage;

    public RoomPatientFactory(IServiceProvider provider, Storage<RoomPatient> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public RoomPatient CreateNewRoomPatient(DateTime datePlaced, DateTime? dateDischarged)
    {
        var roomPatient = _provider.GetRequiredService<RoomPatient>();
        roomPatient.DatePlaced = datePlaced;
        roomPatient.DateDischarged = dateDischarged;
        
        _storage.RegisterNew(roomPatient);
        
        return roomPatient;
    }
}