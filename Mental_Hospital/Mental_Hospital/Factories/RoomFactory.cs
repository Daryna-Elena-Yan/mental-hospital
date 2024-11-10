using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class RoomFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Room> _storage;
    private readonly RoomValidator _validator;

    public RoomFactory(IServiceProvider provider, Storage<Room> storage, RoomValidator validator)
    {
        _provider = provider;
        _storage = storage;
        _validator = validator;
    }

    public Room CreateNewRoom(int capacity)
    {
        var room = _provider.GetRequiredService<Room>();
        room.IdRoom = Guid.NewGuid();
        room.Capacity = capacity;
        
        _validator.ValidateAndThrow(room);
        _storage.RegisterNew(room);
        
        return room;
    }
}