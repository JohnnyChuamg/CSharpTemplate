using System.Security.Claims;
using Microsoft.Extensions.Options;

using Infrastructure.Abstraction;
using Infrastructure.Commons;
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
        services.AddSingleton(_ => new Snowflake(config.MachineId ?? 0L, config.DataCenterId ?? 0L));
    }
}