using Mental_Hospital.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace Mental_Hospital;

public static class ServiceProviderExtensions
{
    public static IStorage? FindStorage(this IServiceProvider provider, Type itemType)
    {
        while (itemType.BaseType != typeof(object))
        {
            itemType = itemType.BaseType;
        }

        var inStorageType = typeof(Storage<>);
        var makeGenericType = inStorageType.MakeGenericType(itemType);
        return provider.GetService(makeGenericType) as IStorage;
    }
}