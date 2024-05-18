using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using FluentValidation;

using Infrastructure.Contracts.ResultContracts;
using Infrastructure.Mvc.Extensions;

namespace Infrastructure.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ExceptionHandlerFilterAttribute(
    ILogger<ExceptionHandlerFilterAttribute> logger,
    IHostingEnvironment environment) : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, context.Exception.ToString());
        context.ExceptionHandled = true;
        var exception = context.Exception;
        switch (exception)
        {
            case ValidationException validationException:
                context.HttpContext.Response.StatusCode = ResultHelper.ConvertHttpStatusCode(ResultCode.InvalidInput);
                context.Result = Result.InvalidInput(validationException.Errors.ToString()).ToActionResult();
                break;
            case ArgumentException argumentException:
                context.HttpContext.Response.StatusCode = ResultHelper.ConvertHttpStatusCode(ResultCode.InvalidInput);
                context.Result = Result.InvalidInput(argumentException.Message).ToActionResult();
                break;
            default:
                context.HttpContext.Response.StatusCode =
                    ResultHelper.ConvertHttpStatusCode(ResultCode.InternalServerError);
                var message = environment.IsDevelopment()
                    ? context.Exception.ToString()
                    : context.Exception.Message;
                context.Result = Result.InternalServerError(message).ToActionResult();
                break;
        }
    }
}