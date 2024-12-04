using FluentValidation;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class PrescriptionFactory : IFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Prescription> _storage;
    private readonly PrescriptionValidator _validator;

    public PrescriptionFactory(IServiceProvider provider, Storage<Prescription> storage, PrescriptionValidator validator)
    {
        _provider = provider;
        _storage = storage;
        _validator = validator;
    }

    public Prescription CreateNewPrescription(Appointment? appointment, string name, int quantity, Decimal dosage, string description)
    {
        // TODO check if appointment exists, if not return null 
        
        var prescription = _provider.GetRequiredService<Prescription>();
        prescription.Id = Guid.NewGuid();
        prescription.Name = name;
        prescription.Quantity = quantity;
        prescription.Dosage = dosage;
        prescription.Description = description;
        prescription.Appointment = appointment;
        
        _validator.ValidateAndThrow(prescription);
        _storage.RegisterNew(prescription);
        
        return prescription;
    }
}