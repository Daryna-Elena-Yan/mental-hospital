using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital.Factories;

public class PrescriptionFactory
{
    private readonly IServiceProvider _provider;
    private readonly Storage<Prescription> _storage;

    public PrescriptionFactory(IServiceProvider provider, Storage<Prescription> storage)
    {
        _provider = provider;
        _storage = storage;
    }

    public Prescription CreateNewPrescription(Appointment appointment, string name, int quantity, Decimal dosage, string description)
    {
        // TODO check if appointment exists, if not return null 
        
        var prescription = _provider.GetRequiredService<Prescription>();
        prescription.IdPrescription = Guid.NewGuid();
        prescription.Name = name;
        prescription.Quantity = quantity;
        prescription.Dosage = dosage;
        prescription.Description = description;
        prescription.Appointment = appointment;
        
        _storage.RegisterNew(prescription);
        
        return prescription;
    }
}