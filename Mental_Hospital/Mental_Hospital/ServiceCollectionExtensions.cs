﻿using FluentValidation;
using Mental_Hospital.Factories;
using Mental_Hospital.Models;
using Mental_Hospital.Models.Light;
using Mental_Hospital.Models.Severe;
using Mental_Hospital.Services;
using Mental_Hospital.Storages;
using Mental_Hospital.Validators;
using Microsoft.Extensions.DependencyInjection;


namespace Mental_Hospital;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection MentalHospitalSetup(this ServiceCollection serviceCollection)
    {
        //registration of services
        serviceCollection.AddTransient<Nurse>();
        serviceCollection.AddTransient<Therapist>();
        serviceCollection.AddTransient<Patient>();
        
        serviceCollection.AddSingleton<Storage<Therapist>>();
        serviceCollection.AddSingleton<Storage<Nurse>>();
        serviceCollection.AddSingleton<Storage<Patient>>();
        serviceCollection.AddSingleton<PersonFactory>();
        serviceCollection.AddSingleton<IStorageAction<Patient>, PatientStorageActions>();
        serviceCollection.AddSingleton<IStorageAction<Therapist>, TherapistStorageActions>();
        serviceCollection.AddSingleton<IStorageAction<Nurse>, NurseStorageActions>();

        serviceCollection.AddTransient<Appointment>();
        serviceCollection.AddSingleton<Storage<Appointment>>();
        serviceCollection.AddSingleton<AppointmentFactory>();
        serviceCollection.AddSingleton<IStorageAction<Appointment>, AppointmentStorageActions>();
        
        serviceCollection.AddTransient<Prescription>();
        serviceCollection.AddSingleton<Storage<Prescription>>();
        serviceCollection.AddSingleton<PrescriptionFactory>();
        serviceCollection.AddSingleton<IStorageAction<Prescription>, PrescriptionStorageActions>();
        
        serviceCollection.AddTransient<Room>();
        serviceCollection.AddSingleton<Storage<Room>>();
        serviceCollection.AddSingleton<RoomFactory>();
        serviceCollection.AddSingleton<IStorageAction<Room>, RoomStorageActions>();
        
        serviceCollection.AddTransient<Equipment>();
        serviceCollection.AddSingleton<Storage<Equipment>>();
        serviceCollection.AddSingleton<EquipmentFactory>();
        serviceCollection.AddSingleton<IStorageAction<Equipment>, EquipmentStorageActions>();
        
        serviceCollection.AddTransient<RoomPatient>();
        serviceCollection.AddSingleton<Storage<RoomPatient>>();
        serviceCollection.AddSingleton<RoomPatientFactory>();
        serviceCollection.AddSingleton<IStorageAction<RoomPatient>, RoomPatientStorageActions>();

        
        serviceCollection.AddTransient<LightAnxiety>();
        serviceCollection.AddTransient<LightMood>();
        serviceCollection.AddTransient<LightPsychotic>();
        serviceCollection.AddTransient<SevereAnxiety>();
        serviceCollection.AddTransient<SevereMood>();
        serviceCollection.AddTransient<SeverePsychotic>();
        serviceCollection.AddSingleton<Storage<Diagnosis>>();
        serviceCollection.AddSingleton<DiagnosisFactory>();
        serviceCollection.AddSingleton<IStorageAction<Diagnosis>, DiagnosisStorageActions>();
        serviceCollection.AddSingleton<FileService>();
        
        
        //adds all valigators!!!
        serviceCollection.AddValidatorsFromAssemblyContaining<DiagnosisValidator>();
        


        //TODO implemetns throug assembly using one common interface
        
        
        return serviceCollection;
    }
}