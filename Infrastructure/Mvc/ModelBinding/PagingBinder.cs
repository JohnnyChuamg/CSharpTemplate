using Infrastructure.Contracts.ResultContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Mvc.ModelBinding;

public class PagingBinder(PagingBinderOptions options) : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(bindingContext.ModelName);
        var queryPaging = new QueryPaging();
        var offset = GetQuery(bindingContext.ActionContext.HttpContext.Request.Query,
            options.OffsetBindingName ?? string.Empty,
            "Offset");
        if (!string.IsNullOrWhiteSpace(offset) && int.TryParse(offset, out var intOffset))
        {
            queryPaging.Offset = intOffset;
        }

        var limit = GetQuery(bindingContext.ActionContext.HttpContext.Request.Query,
            options.LimitBindingName ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(limit) && int.TryParse(limit, out var intLimit))
        {
            queryPaging.Limit = intLimit;
        }

        bindingContext.Result = ModelBindingResult.Success(queryPaging);
        return Task.CompletedTask;
    }

    private string? GetQuery(IQueryCollection queryCollection, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (queryCollection.TryGetValue(key, out var value))
            {
                return value.ToString();
            }
        }

        return null;
    }
}