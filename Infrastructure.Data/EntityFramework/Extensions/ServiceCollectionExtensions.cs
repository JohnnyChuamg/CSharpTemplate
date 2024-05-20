using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Infrastructure.Abstraction;
using Infrastructure.Data.EntityFramework.DbContext;
using Infrastructure.Data.EntityFramework.Enums;
using Infrastructure.Data.EntityFramework.Options;

namespace Infrastructure.Data.EntityFramework.Extensions;
#nullable disable
public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddDbContext(this IServiceCollection services,
        Action<DbContextOption> optionsAction)
    {
        return AddDbContext(services, optionsAction, ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services,
        Action<DbContextOption> optionsAction, ServiceLifetime serviceLifetime)
    {
        var options = new DbContextOption();
        optionsAction.Invoke(options);
        ArgumentNullException.ThrowIfNull(options.EntityMapSettings);
        if (!options.DbContextSettings?.Any() ?? false)
        {
            throw new ArgumentNullException(nameof(DbContextSetting));
        }

        services.TryAdd(new ServiceDescriptor(
            typeof(IGenericContainer<ClaimsPrincipal>),
            typeof(GenericContainer<ClaimsPrincipal>),
            ServiceLifetime.Scoped));

        foreach (var dbContextSetting in options.DbContextSettings)
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(RelationalDbContext),
                    (IServiceProvider serviceProvider)
                        => new RelationalDbContext(
                            dbContextSetting.OptionsBuilder.Options,
                            options.EntityMapSettings,
                            serviceProvider.GetRequiredService<IGenericContainer<ClaimsPrincipal>>())
                        {
                            NodeType = dbContextSetting.NodeType
                        }, serviceLifetime));
        }

        services.Add(
            new ServiceDescriptor(typeof(ServiceResolver<NodeType, RelationalDbContext>),
                serviceProvider =>
                    (ServiceResolver<NodeType, RelationalDbContext>)(type =>
                        serviceProvider.GetServices<RelationalDbContext>()
                            .FirstOrDefault(context => context.NodeType == type) ??
                        throw new InvalidOperationException()), serviceLifetime));

        services.Add(new ServiceDescriptor(typeof(DbContextIngress), typeof(DbContextIngress), serviceLifetime));
        return services;
    }
}