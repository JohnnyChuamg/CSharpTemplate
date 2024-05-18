using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Mvc.ModelBinding;

public class FromQueryPagingAttribute
{
    public BindingSource BindingSource { get; } = PagingBindingSource.FromQueryPaging;
}