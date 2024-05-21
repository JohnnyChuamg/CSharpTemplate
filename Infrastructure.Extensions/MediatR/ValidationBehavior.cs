using FluentValidation;
using MediatR;

namespace Infrastructure.Extensions.MeidatR;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationFailures = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = validationFailures.SelectMany(s => s.Errors).Where(w => w != null);
        //var errors = validators.Select(s => s.Validate(context))
        //    .SelectMany(s => s.Errors).Where(w => w != null);

        if (errors.Any())
        {
            throw new ValidationException(errors);
        }

        var response = await next();
        return response;
    }
}