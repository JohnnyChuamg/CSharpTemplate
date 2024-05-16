using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MediatR;

using Infrastructure.Contracts.ResultContracts;
using Infrastructure.Data.Repositories;
using WorkLog.Infrastructure.Enums;

namespace WorkLog.Application.Services.Users;

public class QueryUsers : IRequest<Result<IEnumerable<QueryUsersResponse>>>
{
    public required Expression<Func<Domain.Entities.User, bool>> Predicate { get; init; }

    public class GetUsersQueryHandler(IRepositoryFactory repositoryFactory)
        : IRequestHandler<QueryUsers, Result<IEnumerable<QueryUsersResponse>>>
    {
        public async Task<Result<IEnumerable<QueryUsersResponse>>> Handle(QueryUsers request,
            CancellationToken cancellationToken)
        {
            using var repo = repositoryFactory.Create<Domain.Entities.User>();
            Expression<Func<Domain.Entities.User, bool>> predicate = predicate => predicate.Status == Status.Enable;

            predicate = predicate.And(request.Predicate);

            var result = (await repo.QueryAsync(predicate, cancellationToken))
                .AsNoTracking()
                .ToList()
                .Select(QueryUsersResponse.FromUser);
            return await Result.SuccessAsync(result);
        }
    }
}