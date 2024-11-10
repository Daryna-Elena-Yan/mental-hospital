using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class EquipmentFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Equipment> _storage;
    private readonly EquipmentValidator _validator;

    public EquipmentFactory(IServiceProvider provider, Storage<Equipment> storage, EquipmentValidator validator)
    {
        _provider = provider;
        _storage = storage;
        _validator = validator;
    }

    public Equipment CreateNewEquipment(string name, DateTime expirationDate)
    {
        var equipment = _provider.GetRequiredService<Equipment>();
        equipment.IdEquipment = Guid.NewGuid();
        equipment.Name = name;
        equipment.ExpirationDate = expirationDate;
        
        _validator.ValidateAndThrow(equipment);
        _storage.RegisterNew(equipment);
        
        return equipment;
    }
}