using Infrastructure.Caching.Redis;
using Infrastructure.Data.EntityFramework;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using WorkLog.Application.Infrastructures.Contracts;

namespace WorkLog.Api.Diagnostics;

public class ServiceHealthCheck(
    DbContextIngress dbContextIngress,
    IOptions<ConfigSettings> options,
    IRedis redis,
    IHttpContextAccessor httpContextAccessor) : IHealthCheck
{
    private readonly string _version = string.IsNullOrWhiteSpace(options.Value.Version) ? "v1" : options.Value.Version;
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            if(cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();

            var connectMaster = dbContextIngress.Master.Database.CanConnectAsync(cancellationToken);
            if (!await connectMaster) throw new Exception("Unable to connect to master database");

            var connectSlave = dbContextIngress.Slave.Database.CanConnectAsync(cancellationToken);
            if (!await connectSlave) throw new Exception("Unable to connect to slave database");

            await redis.PingAsync(cancellationToken: cancellationToken);
            
            return HealthCheckResult.Healthy($"ok\r\n{_version}");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(e.ToString());
        }
    }
}