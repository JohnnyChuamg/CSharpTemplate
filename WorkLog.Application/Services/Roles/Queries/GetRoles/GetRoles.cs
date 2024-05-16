using MediatR;

namespace WorkLog.Application.Services.Roles;

public class GetRoles : IRequest<IEnumerable<GetRolesResponse>>
{
    public class GetRolesHandle : IRequestHandler<GetRoles, IEnumerable<GetRolesResponse>>
    {
        public Task<IEnumerable<GetRolesResponse>> Handle(GetRoles request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}