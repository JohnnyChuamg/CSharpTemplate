using Infrastructure.Data.EntityFramework;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using WorkLog.Application.Infrastructures.Contracts;

namespace WorkLog.Api.Diagnostics;

public class ServiceHealthCheck(
    DbContextIngress dbContextIngress,
    IOptions<ConfigSettings> options,
    IHttpContextAccessor httpContextAccessor) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}