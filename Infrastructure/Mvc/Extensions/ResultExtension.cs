using Infrastructure.Contracts.ResultContracts;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Mvc.Extensions;

public static class ResultExtension
{
#nullable disable
    public static ActionResult ToActionResult(this Result result)
        => new ObjectResult(result)
        {
            StatusCode = result.HttpStatusCode,
            Value = result.Code == ResultCode.SuccessNoContent ? null : result
        };

    public static ActionResult ToActionResult<T>(this Result<T> result)
        => new ObjectResult(result)
        {
            DeclaredType = typeof(T),
            StatusCode = result.HttpStatusCode,
            Value = result.Code == ResultCode.SuccessNoContent ? null : result
        };
}