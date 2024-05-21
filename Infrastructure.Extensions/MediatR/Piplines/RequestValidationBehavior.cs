using FluentValidation;
using MediatR;

namespace Infrastructure.Extensions.MeidatR.Piplines;

public class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var list = validators.Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors).Where(w => w != null).ToList();

        if ((list?.Count ?? 0) != 0)
        {
            throw new ValidationException(list);
        }

        return next();
    }
}