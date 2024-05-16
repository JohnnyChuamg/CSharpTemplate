using System.Reflection;
using WorkLog.Application.Facades;

namespace WorkLog.Api.InjectionConfigs;

public class FacadeConfig
{
    private readonly IServiceCollection _services;

    public FacadeConfig(IServiceCollection services)
    {
        _services = services;

        var assembly = Assembly.GetAssembly(typeof(IFacades));
        var types = assembly?.GetTypes()
            .Where(t => t.IsClass
                        && t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 1
                        && typeof(IFacades).IsAssignableFrom(t)
            ).ToArray() ?? [];
        if (types.Length == 0) return;
        RegisterServices(types);
    }

    private void RegisterServices(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var instance = type.GetInterfaces().FirstOrDefault(f => f.Name != nameof(IFacades));
            if (instance == null)
            {
                _services.AddScoped(type);
                continue;
            }

            _services.AddScoped(instance, type);
        }
    }
}