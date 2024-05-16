using System.Security.Claims;
using Microsoft.Extensions.Options;

using Infrastructure.Abstraction;
using Infrastructure.Extensions.DependencyInjection;
using Infrastructure.Serialization.JsonSerializers;
using WorkLog.Application.Infrastructures.Contracts;

namespace WorkLog.Api.InjectionConfigs;

[Injection]
public class CommonConfig
{
    public CommonConfig(IServiceCollection services, IOptions<ConfigSettings> options)
    {
        var config = options.Value;

        services.AddScoped<IGenericContainer<ClaimsPrincipal>, GenericContainer<ClaimsPrincipal>>();
        services.AddSingleton<IJsonSerializer, JsonSerializer>();
        // services.AddSingleton(provider => new Snowflake(config.MachineId, config.DataCenterId));
    }
}