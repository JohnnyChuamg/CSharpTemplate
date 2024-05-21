using System.Reflection;
using Infrastructure.Extensions.DependencyInjection;
using Infrastructure.Extensions.MeidatR.Piplines;
using MediatR;
using WorkLog.Application.Services;
namespace WorkLog.Api.InjectionConfigs;

[Injection]
public class MediatrConfig
{
    public MediatrConfig(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(IService).GetTypeInfo().Assembly));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>),typeof(RequestValidationBehavior<,>))
    }
}

