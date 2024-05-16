using Infrastructure.Contracts.ResultContracts;
using MediatR;

namespace WorkLog.Application.Services.Users;

public class CreateUser : IRequest<Result>
{
    public class CreateUserHandle : IRequestHandler<CreateUser, Result>
    {
        public Task<Result> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}