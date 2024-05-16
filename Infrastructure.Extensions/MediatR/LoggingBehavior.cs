using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Extensions.MeidatR;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var stopWatch = new Stopwatch();
        logger.LogInformation($"Handling {typeof(TRequest).Name}");
        stopWatch.Start();
        var response = await next();
        stopWatch.Stop();
        logger.LogInformation($"Handling {typeof(TResponse).Name} in {stopWatch.ElapsedMilliseconds} ms");
        stopWatch.Reset();
        return response;
    }
}