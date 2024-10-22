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

    public Prescription CreateNewPrescription(int idPrescription, string name, int quantity, Decimal dosage, string description)
    {
        var prescription = _provider.GetRequiredService<Prescription>();
        prescription.IdPrescription = idPrescription;
        prescription.Name = name;
        prescription.Quantity = quantity;
        prescription.Dosage = dosage;
        prescription.Description = description;
        
        _storage.RegisterNew(prescription);
        
        return prescription;
    }
}