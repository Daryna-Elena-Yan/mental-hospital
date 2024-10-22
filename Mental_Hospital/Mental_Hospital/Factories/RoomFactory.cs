using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class RoomFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Room> _storage;

    public RoomFactory(IServiceProvider provider, Storage<Room> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Room CreateNewRoom(int idRoom, int quantity)
    {
        var room = _provider.GetRequiredService<Room>();
        room.IdRoom = idRoom;
        room.Quantity = quantity;
        
        _storage.RegisterNew(room);
        
        return room;
    }
}