using MediatR;

namespace WorkLog.Application.Services.Departments;

public class GetDepartments : IRequest<IEnumerable<GetDepartmentsResponse>>
{
    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartments, IEnumerable<GetDepartmentsResponse>>
    {
        public Task<IEnumerable<GetDepartmentsResponse>> Handle(GetDepartments query,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}