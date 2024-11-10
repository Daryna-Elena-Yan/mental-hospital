using FluentValidation;
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
        RegisterFactories(serviceCollection);
        RegisterModels(serviceCollection);
        RegisterStorages(serviceCollection);
        RegisterStorageActions(serviceCollection);
        serviceCollection.AddValidatorsFromAssemblyContaining<DiagnosisValidator>();  //adds all validators!!!
        serviceCollection.AddSingleton<StorageManager>();
        
        return serviceCollection;
    }

    private static void RegisterStorageActions(ServiceCollection serviceCollection)
    {
        var type = typeof(IStorageAction<>);
        var actionTypes = type.Assembly.GetTypes()
            .Where(p => p.GetInterfaces().Any(x => x.IsGenericType &&
                                                   x.GetGenericTypeDefinition() == typeof(IStorageAction<>)));
        foreach (var actionType in actionTypes)
        {
            var makeGenericType = actionType.GetInterfaces().First();  
            serviceCollection.AddSingleton(makeGenericType, actionType);
           
        }
    }

    private static void RegisterStorages(ServiceCollection serviceCollection)
    {
        var type = typeof(IEntity);
        var storageTypes = type.Assembly.GetTypes()
            .Where(p => type.IsAssignableFrom(p) && p.BaseType == typeof(object));
        foreach (var storageType in storageTypes)
        {
            var inStorageType = typeof(Storage<>);
            var makeGenericType = inStorageType.MakeGenericType(storageType);
            serviceCollection.AddSingleton(makeGenericType);
            serviceCollection.AddSingleton<IStorage>(s => s.GetRequiredService(makeGenericType) as IStorage);
        }
    }

    private static void RegisterModels(ServiceCollection serviceCollection)
    {
        var type = typeof(IEntity);
        var entityTypes = type.Assembly.GetTypes()
            .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);
        foreach (var entityType in entityTypes)
        {
            serviceCollection.AddTransient(entityType);
        }
    }

    private static void RegisterFactories(ServiceCollection serviceCollection)
    {
        var type1 = typeof(IFactory);
        var factoryTypes = type1.Assembly.GetTypes()
            .Where(p => type1.IsAssignableFrom(p) && !p.IsAbstract);
        foreach (var fType in factoryTypes)
        {
            serviceCollection.AddSingleton(fType);
        }
    }
}