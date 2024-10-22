using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection MentalHospitalSetup(this ServiceCollection serviceCollection)
    {
        //part of innitializing
        serviceCollection.AddTransient<Patient>();
        serviceCollection.AddSingleton<Storage<Person>>();
        serviceCollection.AddSingleton<PersonFactory>();
        
        serviceCollection.AddTransient<Appointment>();
        serviceCollection.AddSingleton<Storage<Appointment>>();
        serviceCollection.AddSingleton<AppointmentFactory>();
        
        serviceCollection.AddTransient<Prescription>();
        serviceCollection.AddSingleton<Storage<Prescription>>();
        serviceCollection.AddSingleton<PrescriptionFactory>();
        
        serviceCollection.AddTransient<Room>();
        serviceCollection.AddSingleton<Storage<Room>>();
        serviceCollection.AddSingleton<RoomFactory>();
        
        serviceCollection.AddTransient<Equipment>();
        serviceCollection.AddSingleton<Storage<Equipment>>();
        serviceCollection.AddSingleton<EquipmentFactory>();
        
        serviceCollection.AddTransient<RoomPatient>();
        serviceCollection.AddSingleton<Storage<RoomPatient>>();
        serviceCollection.AddSingleton<RoomPatientFactory>();
        
        return serviceCollection;
    }
}