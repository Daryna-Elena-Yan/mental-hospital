using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class EquipmentFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Equipment> _storage;

    public EquipmentFactory(IServiceProvider provider, Storage<Equipment> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Equipment CreateNewEquipment(string name, DateTime expirationDate)
    {
        var equipment = _provider.GetRequiredService<Equipment>();
        equipment.Name = name;
        equipment.ExpirationDate = expirationDate;
        
        _storage.RegisterNew(equipment);
        
        return equipment;
    }
}