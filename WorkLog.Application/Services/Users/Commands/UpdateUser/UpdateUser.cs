using Infrastructure.Contracts.ResultContracts;
using MediatR;

namespace WorkLog.Application.Services.Users;

public class UpdateUser : IRequest<Result>
{
    public long Id { get; set; }

    public class UpdateUserHandle : IRequestHandler<UpdateUser, Result>
    {
        public Task<Result> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}