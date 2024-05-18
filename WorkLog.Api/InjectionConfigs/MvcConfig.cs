using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using WorkLog.Api.Diagnostics;
using WorkLog.Application.Infrastructures.Contracts;
using WorkLog.Application.Services;

namespace WorkLog.Api.InjectionConfigs;

public class MvcConfig
{
    public MvcConfig(IServiceCollection services, IOptions<ConfigSettings> options)
    {
        services.AddCors()
            .AddHealthChecks()
            .AddCheck<ServiceHealthCheck>(nameof(ServiceHealthCheck))
            ;
        services.AddHttpContextAccessor()
            .AddHttpClient()
            .AddControllers(option =>
            {
                
            })
            ;
        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(typeof(IService).GetTypeInfo().Assembly);
        

    }
}