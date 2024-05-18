using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Infrastructure.Mvc.ModelBinding;

public class PagingBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest)
    : BindingSource(id, displayName,
        isGreedy, isFromRequest)
{
    public static readonly BindingSource FromQueryPaging =
        new PagingBindingSource("FromQueryPaging", "FromQueryPaging", isGreedy: true, isFromRequest: true);

    public override bool CanAcceptDataFrom(BindingSource bindingSource)
    {
        return bindingSource == this;
    }
}