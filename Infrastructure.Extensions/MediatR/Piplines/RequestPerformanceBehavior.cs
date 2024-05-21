using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions.MeidatR.Piplines;

public class RequestPerformanceBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();
        var result = await next();
        if (_timer.ElapsedMilliseconds <= 500) return result;
        var name = typeof(TRequest).Name;
        logger.LogWarning("Long running request: {Name} ({ElapsedMilliseconds} milliseconds {@Request})", name,
            _timer.ElapsedMilliseconds, request);

        return result;
    }
}