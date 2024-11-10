using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class RoomFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Room> _storage;

    public RoomFactory(IServiceProvider provider, Storage<Room> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Room CreateNewRoom(int capacity)
    {
        var room = _provider.GetRequiredService<Room>();
        room.IdRoom = Guid.NewGuid();
        room.Capacity = capacity;
        
        _storage.RegisterNew(room);
        
        return room;
    }
}