using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDependencyInjections(this IServiceCollection? services)
        => services.ConfigureDependencyInjections(GetInjectionTypes());

    public static IServiceCollection ConfigureDependencyInjections<TConfig>(this IServiceCollection? services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
        {
            throw new NullReferenceException("EntryAssembly not found");
        }

        var configTypes = (
            from t in entryAssembly.GetTypes()
            where t.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 1
                  && typeof(TConfig).IsAssignableFrom(t)
            select t
        ).ToArray();
        return services.ConfigureDependencyInjections(configTypes);
    }

    public static IServiceCollection ConfigureDependencyInjections(this IServiceCollection? services,
        params Type[]? configTypes)
    {
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(configTypes);

        if (configTypes.Length == 0)
        {
            return services;
        }

        var array = (
            from t in configTypes
            where t.GetConstructors().Length == 1
            orderby t.GetCustomAttribute<InjectionAttribute>()?.Index ?? 0 descending, t.Name
            select t
        ).ToArray();

        foreach (var type in array)
        {
            var list = new List<object>();
            var parameters = type.GetConstructors()[0].GetParameters();
            foreach (var parameterInfo in parameters)
            {
                if (parameterInfo.ParameterType == typeof(IServiceCollection))
                {
                    list.Add(services);
                    continue;
                }

                var requiredService = services.GetRequiredService(parameterInfo.ParameterType);
                list.Add(requiredService);
            }

            if (list.Count != 0)
            {
                Activator.CreateInstance(type, list.ToArray());
            }
        }

        return services;
    }

    public static T GetRequiredService<T>(this IServiceCollection services) where T : class
        => services.BuildServiceProvider().GetRequiredService<T>();

    public static object GetRequiredService(this IServiceCollection services, Type serviceType)
        => services.BuildServiceProvider().GetRequiredService(serviceType);

    public static T? GetService<T>(this IServiceCollection services) where T : class
        => services.BuildServiceProvider().GetService<T>();

    public static object? GetService(this IServiceCollection services, Type serviceType)
        => services.BuildServiceProvider().GetService(serviceType);

    private static Type[] GetInjectionTypes()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly == null)
        {
            throw new NullReferenceException("EntryAssembly not found");
        }

        return (
            from t in entryAssembly.GetTypes()
            where t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 1
                  && t.CustomAttributes.Any(a => a.AttributeType == typeof(InjectionAttribute))
            select t
        ).ToArray();
    }
}