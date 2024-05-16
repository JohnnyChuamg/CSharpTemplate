using MediatR;
using Infrastructure.Contracts.ResultContracts;
using Infrastructure.Data.Repositories;

namespace WorkLog.Application.Services.Users;

public class GetUserDetail : IRequest<Result<GetUserDetailResponse?>>
{
    public long Id { get; init; }

    public class GetUserDetailQueryHandle(IRepositoryFactory repositoryFactory)
        : IRequestHandler<GetUserDetail, Result<GetUserDetailResponse?>>
    {

        public async Task<Result<GetUserDetailResponse?>> Handle(GetUserDetail request,
            CancellationToken cancellationToken)
        {
            var repo = repositoryFactory.Create<Domain.Entities.User>();

            var result = (await repo.QueryAsync(s => s.Id == request.Id, cancellationToken))
                .ToList().Select(GetUserDetailResponse.FromUser).FirstOrDefault();

            return await Result.SuccessAsync(result);
        }
    }
}