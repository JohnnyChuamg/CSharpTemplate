using Infrastructure.Contracts.ResultContracts;
using MediatR;

namespace WorkLog.Application.Services.Users;

public class UpdateUserStatus : IRequest<Result>
{
    public int Id { get; set; }

    public class UpdateUserStatusHandle : IRequestHandler<UpdateUserStatus, Result>
    {
        public Task<Result> Handle(UpdateUserStatus request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}