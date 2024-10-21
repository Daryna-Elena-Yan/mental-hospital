using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection MentalHospitalSetup(this ServiceCollection serviceCollection)
    {
        //part of innitializing
        serviceCollection.AddSingleton<PersonStorage>();
        serviceCollection.AddTransient<Patient>();
        serviceCollection.AddTransient<PersonFactory>();
        return serviceCollection;
    }
}