using Infrastructure.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Caching.Redis;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddRedis<TProvider>(this IServiceCollection services, RedisOptions options)
        where TProvider : IRedis
    {
        return AddRedis<TProvider>(services, options, ServiceLifetime.Singleton);
    }

    public static IServiceCollection AddRedis<TProvider>(this IServiceCollection services, Action<RedisOptions> options)
        where TProvider : IRedis
    {
        var redisOptions = new RedisOptions();
        options(redisOptions);
        return AddRedis<TProvider>(services, redisOptions);
    }

    private static IServiceCollection AddRedis<TProvider>(IServiceCollection services, RedisOptions options,
        ServiceLifetime lifetime) where TProvider : IRedis
    {
        var descriptor = ServiceDescriptor.Describe(typeof(IRedis), delegate(IServiceProvider provider)
        {
            var logger = provider.GetRequiredService<ILogger<TProvider>>();
            options.Logger = logger;
            return Activator.CreateInstance(typeof(TProvider), options) ?? throw new InvalidOperationException();
        }, lifetime);
        services.Add(descriptor);
        services.AddLogging();
        services.TryAdd(new ServiceDescriptor(typeof(ServiceResolver<string, IRedis>),
            serviceProvider => (ServiceResolver<string, IRedis>)(instance =>
                serviceProvider.GetServices<IRedis>().FirstOrDefault(f => f.Instance == instance) ?? throw new InvalidOperationException()), lifetime)
        );
        return services;
    }
}