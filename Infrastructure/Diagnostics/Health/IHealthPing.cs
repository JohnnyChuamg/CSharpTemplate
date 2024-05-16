namespace Infrastructure.Diagnostics.Health;

public interface IHealthPing
{
    TimeSpan Ping(int timeout = 5000);

    Task<TimeSpan> PingAsync(int timeout = 5000, CancellationToken cancellationToken = default);
}