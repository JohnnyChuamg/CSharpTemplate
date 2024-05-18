using Infrastructure.Contracts.ResultContracts;

namespace Infrastructure.Mvc.Extensions;

public static class QueryPagingExtension
{
    public static Paging ToPaging(this QueryPaging paging)
        => new Paging(paging.Offset, paging.Limit);
}