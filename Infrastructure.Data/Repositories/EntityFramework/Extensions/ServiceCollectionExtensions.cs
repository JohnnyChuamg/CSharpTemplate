using Infrastructure.Data.EntityFramework.Options;
using Infrastructure.Data.Repositories.EntityFramework.Options;
using Infrastructure.Data.EntityFramework.Extensions;
using Infrastructure.Data.Repositories.EntityFramework.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data.Repositories.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services,
        Action<RepositoryFactoryOptions>? optionsAction, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        var options = new RepositoryFactoryOptions();
        optionsAction?.Invoke(options);

        ArgumentNullException.ThrowIfNull(options.DbContextOptions);
        ArgumentNullException.ThrowIfNull(options.DbContextOptions.EntityMapSettings);
        ArgumentNullException.ThrowIfNull(options.DbContextOptions.DbContextSettings);

        services.AddDbContext(delegate(DbContextOption option)
        {
            option.EntityMapSettings = options.DbContextOptions.EntityMapSettings;
            option.DbContextSettings = options.DbContextOptions.DbContextSettings;
        }, ServiceLifetime.Transient);

        services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), typeof(RepositoryFactory), serviceLifetime));
        return services;
    }
}