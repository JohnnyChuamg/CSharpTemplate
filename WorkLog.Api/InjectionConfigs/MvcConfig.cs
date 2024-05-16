using Microsoft.Extensions.Options;
using WorkLog.Application.Infrastructures.Contracts;

namespace WorkLog.Api.InjectionConfigs;

public class MvcConfig
{
    public MvcConfig(IServiceCollection services, IOptions<ConfigSettings> options)
    {
        services.AddCors()
            .AddHealthChecks()
            ;
    }
}