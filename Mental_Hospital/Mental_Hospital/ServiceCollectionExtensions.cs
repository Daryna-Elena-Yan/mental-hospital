using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection MentalHospitalSetup(this ServiceCollection serviceCollection)
    {
        //registration of services
        serviceCollection.AddTransient<Nurse>();
        serviceCollection.AddTransient<Patient>();
        serviceCollection.AddTransient<Therapist>();
        serviceCollection.AddTransient<Patient>();
        
        serviceCollection.AddSingleton<Storage<Person>>();
        serviceCollection.AddSingleton<PersonFactory>();
        serviceCollection.AddSingleton<IStorageAction<Person>, PatientStorageActions>();
        
        serviceCollection.AddTransient<Appointment>();
        serviceCollection.AddSingleton<Storage<Appointment>>();
        serviceCollection.AddSingleton<AppointmentFactory>();
        serviceCollection.AddSingleton<IStorageAction<Appointment>, AppointmentActions>();
        
        serviceCollection.AddTransient<Prescription>();
        serviceCollection.AddSingleton<Storage<Prescription>>();
        serviceCollection.AddSingleton<PrescriptionFactory>();
        serviceCollection.AddSingleton<IStorageAction<Prescription>, PrescriptionActions>();
        
        serviceCollection.AddTransient<Room>();
        serviceCollection.AddSingleton<Storage<Room>>();
        serviceCollection.AddSingleton<RoomFactory>();
        
        serviceCollection.AddTransient<Equipment>();
        serviceCollection.AddSingleton<Storage<Equipment>>();
        serviceCollection.AddSingleton<EquipmentFactory>();
        
        serviceCollection.AddTransient<RoomPatient>();
        serviceCollection.AddSingleton<Storage<RoomPatient>>();
        serviceCollection.AddSingleton<RoomPatientFactory>();
        serviceCollection.AddSingleton<IStorageAction<RoomPatient>, RoomPatientActions>();

        
        serviceCollection.AddTransient<LightAnxiety>();
        serviceCollection.AddTransient<LightMood>();
        serviceCollection.AddTransient<LightPsychotic>();
        serviceCollection.AddTransient<SevereAnxiety>();
        serviceCollection.AddTransient<SevereMood>();
        serviceCollection.AddTransient<SeverePsychotic>();
        serviceCollection.AddSingleton<Storage<Diagnosis>>();
        serviceCollection.AddSingleton<DiagnosisFactory>();
        serviceCollection.AddSingleton<IStorageAction<Diagnosis>, DiagnosisStorageActions>();
        
        


        //TODO implemetns throug assembly using one common interface
        
        
        return serviceCollection;
    }
}