using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using Infrastructure.Contracts.ResultContracts;
namespace Infrastructure.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomExceptionFilterAttribute(
    ILogger<CustomExceptionFilterAttribute> logger,
    IHostingEnvironment environment) : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, context.Exception.ToString());
        context.HttpContext.Response.ContentType = "application/json";
        context.ExceptionHandled = true;
        var exception = context.Exception;
        switch (exception)
        {
            case ValidationException validationException:
            {
                var validationFailure = validationException.Errors.First();
                if (!Enum.TryParse<ResultCode>(validationFailure.ErrorCode, out var result))
                {
                    result = ResultCode.InvalidInput;
                }

                context.HttpContext.Response.StatusCode = ResultHelper.ConvertHttpStatusCode(result);
                context.Result = new JsonResult(new
                {
                    Code = result,
                    Message = validationFailure.ErrorMessage
                });
                break;
            }
            case ArgumentException argumentException:
                context.HttpContext.Response.StatusCode = ResultHelper.ConvertHttpStatusCode(ResultCode.InvalidInput);
                context.Result = new JsonResult(new
                {
                    Code = ResultCode.InvalidInput,
                    argumentException.Message
                });
                break;
            case HttpRequestException httpRequestException:
            {
                if (httpRequestException.Message == "Request body too large.")
                {
                    context.HttpContext.Response.StatusCode =
                        ResultHelper.ConvertHttpStatusCode(ResultCode.PayloadTooLarge);
                }

                context.Result = new JsonResult(new
                {
                    Code = ResultCode.PayloadTooLarge,
                    httpRequestException.Message
                });
                break;
            }
            default:
            {
                context.HttpContext.Response.StatusCode =
                    ResultHelper.ConvertHttpStatusCode(ResultCode.InternalServerError);

                var message = new Dictionary<string, object>
                {
                    { "Code", ResultCode.InternalServerError },
                    { "Message", context.Exception.Message }
                };

                if (!environment.IsProduction())
                {
                    message.Add("Source", context.Exception.Source ?? string.Empty);
                    message.Add("Stack", context.Exception.StackTrace ?? string.Empty);
                    message.Add("InnerException", context.Exception.InnerException?.ToString() ?? string.Empty);
                }

                context.Result = new JsonResult(message);
                break;
            }
        }
    }
}