using Infrastructure.Contracts.ResultContracts;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Mvc.ModelBinding;

public class PagingBinderProvider(PagingBinderOptions options) : IModelBinderProvider
{
    private readonly PagingBinderOptions _options = options;

    public PagingBinderProvider() : this(new PagingBinderOptions())
    {
    }

    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType != typeof(QueryPaging))
        {
            return null;
        }

        if (context.BindingInfo.BindingSource != PagingBindingSource.FromQueryPaging)
        {
            return null;
        }

        return new PagingBinder(_options);
    }
}